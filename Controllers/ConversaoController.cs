
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversaoController : ControllerBase
    {
        private IEmpresaRepository<Empresa> empresaRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IGenericRepository<ContaDePara> contadeparaRepository;
        private static readonly Encoding LocalEncoding = Encoding.UTF8;
        public ConversaoController(IEmpresaRepository<Empresa> empresaRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
            IGenericRepository<ContaDePara> contadeparaRepository)
        {
            this.empresaRepository = empresaRepository;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;
            this.contadeparaRepository = contadeparaRepository;
        }

        [HttpPost()]
        [Route("converter")]
        [Authorize()]
        public IActionResult Converter(Conversao conversao)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                var empresa = empresaRepository.Get(empresaId);
                MemoryStream memory = new MemoryStream();
                TextWriter textWriter = new StreamWriter(memory);
                string vlPartida = "";
                string brancos = "";
                // Escreve cabecalho
                textWriter.WriteLine("01" + empresa.CodigoFilial.Value.ToString("D7") + empresa.Cnpj + conversao.DataInicial.ToString("ddMMyyyy") + conversao.DataFinal.ToString("ddMMyyyy") + "N0100000118");
                conversao.Partidas.ForEach(partida =>
                {
                    if  (partida.ValorDebito == decimal.Zero)
                            vlPartida = partida.ValorCredito.ToString();
                    if (partida.ValorCredito == decimal.Zero)
                        vlPartida = partida.ValorDebito.ToString();

                    textWriter.WriteLine(string.Concat("02",partida.DataLancamento.ToString("ddMMyyyy"), vlPartida.PadLeft(15, '0'), partida.ContaDebito.PadLeft(7, '0'), partida.ContaCredito.PadLeft(7, '0'), partida.Historico.PadRight(512, ' '), empresa.NomeUsuarioDominio.PadRight(30, ' ') + empresa.CodigoFilial.ToString().PadLeft(7, '0') + partida.CodigoHistorico.PadLeft(7, '0') + brancos.PadLeft(100, ' ')));
                });
                textWriter.Flush(); 
                byte[] bytesInStream = memory.ToArray();
                memory.Close();
                return File(bytesInStream, "application/octet-stream");
            }
            catch (Exception ex)
            {
                return BadRequest("Falha na conversão do arquivo - " + ex.Message);
            }
        }


        [HttpPost()]
        [Route("analise")]
        [Authorize()]
        public async Task<IActionResult> Analise()
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                var arquivo = JsonConvert.DeserializeObject<ArquivoEntrada>(Convert.ToString(Request.Form["arquivoEntrada"]));
                if (arquivo.Id == 0)
                {
                    return BadRequest("Arquivo de entrada não encontrado!");
                }
                if (arquivo.HasMapeamento)
                {
                    if (!contadeparaRepository.Where(x => x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Mapeamento de contas não encontrado!");
                    }
                }
                var lst = new List<PartidaDobrada>();
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Form.Files[0].CopyToAsync(memoryStream);
                    string[] lines = LocalEncoding.GetString(memoryStream.ToArray()).Split("\r\n");
                    foreach (string line in lines)
                    {
                        if (!line.Equals(""))
                        {
                            var l = line.Split(";");
                            var partida = new PartidaDobrada();
                            var partidaCredito = new PartidaDobrada();
                            partida.DataLancamento = Convert.ToDateTime(l[arquivo.ColunaData - 1]);
                            partida.CodigoHistorico = "111";
                            partida.Historico = l[arquivo.ColunaHistorico - 1];
                            partida.CodigoLancamento = l[arquivo.ColunaNLancamento - 1];
                            if (arquivo.IsDebito)
                            {
                                if ((!arquivo.ColunaValorDebito.HasValue) || (!arquivo.ColunaContaDebito.HasValue))
                                {
                                    return BadRequest("O Arquivo selecionado contém valores de débito! Obrigatório informar qual a coluna que contém o valor de débito.");
                                }
                                partida.ValorDebito = Convert.ToDecimal(l[arquivo.ColunaValorDebito.Value - 1]);
                                partida.ValorCredito = 0;
                                if (arquivo.HasMapeamento)
                                {
                                    var conta = contadeparaRepository.Where(x => x.De == l[arquivo.ColunaContaDebito.Value - 1]).FirstOrDefault();
                                    if (conta == null)
                                    {
                                        return BadRequest(string.Concat("Conta ", l[arquivo.ColunaContaDebito.Value - 1], " não encontrada no mapeamento"));
                                    }
                                    partida.ContaDebito = conta.Para;
                                    partidaCredito.ContaCredito = conta.Para;
                                }
                                if (!arquivo.HasMapeamento)
                                {
                                    partida.ContaDebito = l[arquivo.ColunaContaDebito.Value - 1];
                                    partidaCredito.ContaCredito = l[arquivo.ColunaContaDebito.Value - 1];
                                }
                                partida.ContaCredito = arquivo.ContaTransitoria;
                                lst.Add(partida);

                                partidaCredito.DataLancamento = Convert.ToDateTime(l[arquivo.ColunaData - 1]);
                                partidaCredito.CodigoHistorico = "111";
                                partidaCredito.Historico = l[arquivo.ColunaHistorico - 1];
                                partidaCredito.CodigoLancamento = l[arquivo.ColunaNLancamento - 1];
                                partidaCredito.ValorCredito = Convert.ToDecimal(l[arquivo.ColunaValorDebito.Value - 1]);
                                partidaCredito.ValorDebito = 0;
                                partidaCredito.ContaDebito = arquivo.ContaTransitoria;
                                lst.Add(partidaCredito);
                            }

                            
                        }


                    }
                }
                return new JsonResult(lst);
            }
            catch (Exception ex)
            {
                return BadRequest("Falha na conversão do arquivo - " + ex.Message);
            }

        }

        //protected async void Download()
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        StreamWriter objstreamwriter = new StreamWriter(stream);
        //        objstreamwriter.Write("This is the content");
        //        objstreamwriter.Flush();
        //        objstreamwriter.Close();
        //        Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        //        Response.Headers.Add("content-disposition", "attachment;  filename=" + string.Format("{0}_{1}", "teste", DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss")) + ".txt");
        //        Response.ContentType = "application/octet-stream";
        //        var bytes = stream.ToArray();
        //        await Response.Body.WriteAsync(bytes, 0, bytes.Length);
        //    }
        //}
    }
}
