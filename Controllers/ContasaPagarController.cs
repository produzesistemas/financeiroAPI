using ClosedXML.Excel;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContasaPagarController : ControllerBase
    {
        private IGenericRepository<ContasAPagar> genericRepository;
        private IGenericRepository<PlanoContas> planoContasRepository;
        private IContasAPagarRepository<ContasAPagar> contasaPagarRepository;
        private IGenericRepository<Empresa> empresaRepository;
        private static readonly Encoding LocalEncoding = Encoding.UTF8;
        public ContasaPagarController(IGenericRepository<ContasAPagar> genericRepository,
        IContasAPagarRepository<ContasAPagar> contasaPagarRepository,
        IGenericRepository<PlanoContas> planoContasRepository,
        IGenericRepository<Empresa> empresaRepository)
        {
            this.genericRepository = genericRepository;
            this.contasaPagarRepository = contasaPagarRepository;
            this.empresaRepository = empresaRepository;
            this.planoContasRepository = planoContasRepository;
        }
        [HttpPost()]
        [Route("filter")]
        [Authorize()]
        public IActionResult GetByFilter(FilterDefault filter)
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

                Expression<Func<ContasAPagar, bool>> p1, p2, p3, p4, p5, p6;
                var predicate = PredicateBuilder.New<ContasAPagar>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                p2 = p => p.SituacaoContaId == 1;
                predicate = predicate.And(p2);
                if (filter.CategoriaContasAPagarId > 0)
                {
                    p3 = p => p.CategoriaContasAPagarId == filter.CategoriaContasAPagarId;
                    predicate = predicate.And(p3);
                }
                if (filter.FornecedorId > 0)
                {
                    p4 = p => p.FornecedorId == filter.FornecedorId;
                    predicate = predicate.And(p4);
                }
                if (filter.CentroCustoId > 0)
                {
                    p5 = p => p.CentroCustoId == filter.CentroCustoId;
                    predicate = predicate.And(p5);
                }
                if (filter.PlanoContasId > 0)
                {
                    p6 = p => p.PlanoContasId == filter.PlanoContasId;
                    predicate = predicate.And(p6);
                }
                return new JsonResult(contasaPagarRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento das contas: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("pagas")]
        [Authorize()]
        public IActionResult GetPagasByFilter(FilterDefault filter)
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

                Expression<Func<ContasAPagar, bool>> p1, p2, p3, p4, p5, p6;
                var predicate = PredicateBuilder.New<ContasAPagar>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                p2 = p => p.SituacaoContaId == 2;
                predicate = predicate.And(p2);
                if (filter.CategoriaContasAPagarId > 0)
                {
                    p3 = p => p.CategoriaContasAPagarId == filter.CategoriaContasAPagarId;
                    predicate = predicate.And(p3);
                }
                if (filter.FornecedorId > 0)
                {
                    p4 = p => p.FornecedorId == filter.FornecedorId;
                    predicate = predicate.And(p4);
                }
                if (filter.CentroCustoId > 0)
                {
                    p5 = p => p.CentroCustoId == filter.CentroCustoId;
                    predicate = predicate.And(p5);
                }
                if (filter.PlanoContasId > 0)
                {
                    p6 = p => p.PlanoContasId == filter.PlanoContasId;
                    predicate = predicate.And(p6);
                }

                return new JsonResult(contasaPagarRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento das contas: ", ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize()]
        public IActionResult Get(int id)
        {
            try
            {
                return new JsonResult(contasaPagarRepository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest("Conta não encontrada!" + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize()]
        public IActionResult Delete(int id)
        {
            try
            {
                var entityBase = genericRepository.Get(id);
                genericRepository.Delete(entityBase);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        [Route("lancamentoManual")]
        [Authorize()]
        public IActionResult LancamentoManual(ContasAPagarLancamentoManual lancamentoManual)
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

                int n = 1;
                while (n <= lancamentoManual.Quantidade)
                {
                    var conta = new ContasAPagar();
                    conta.ApplicationUserId = id;
                    conta.FornecedorId = lancamentoManual.Fornecedor.Id;
                    if (lancamentoManual.ContaCorrente != null)
                    {
                        conta.ContaCorrenteId = lancamentoManual.ContaCorrente.Id;
                    }
                    if (lancamentoManual.CentroCusto != null)
                    {
                        conta.CentroCustoId = lancamentoManual.CentroCusto.Id;
                    }
                    if (lancamentoManual.PlanoContas != null)
                    {
                        conta.PlanoContasId = lancamentoManual.PlanoContas.Id;
                    }
                    if (lancamentoManual.CategoriaContasAPagar != null)
                    {
                        conta.CategoriaContasAPagarId = lancamentoManual.CategoriaContasAPagar.Id;
                    }
                    conta.CreateDate = DateTime.Now;
                    if (n == 1)
                    {
                        conta.DataVencimento = lancamentoManual.DataVencimento;
                    }
                    else
                    {
                        conta.DataVencimento = lancamentoManual.DataVencimento.AddMonths(n - 1);
                    }
                    conta.EmpresaId = empresaId;
                    conta.SituacaoContaId = 1;
                    conta.ValorOriginal = lancamentoManual.ValorOriginal;
                    conta.Referente = lancamentoManual.Referente;
                    genericRepository.Insert(conta);
                    n++;
                }
                                
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no lançamento manual - " + ex);
            }
        }

        [HttpPost()]
        [Route("baixa")]
        [Authorize()]
        public IActionResult Baixa(ContasAPagar contasAPagar)
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
                var entityBase = genericRepository.Get(contasAPagar.Id);
                if (contasAPagar.Juros.HasValue)
                {
                    entityBase.Juros = contasAPagar.Juros.Value;
                }
                if (contasAPagar.Multa.HasValue)
                {
                    entityBase.Multa = contasAPagar.Multa.Value;
                }
                if (contasAPagar.CategoriaContasAPagarId.HasValue)
                {
                    entityBase.CategoriaContasAPagarId = contasAPagar.CategoriaContasAPagarId.Value;
                }
                if (contasAPagar.PlanoContasId.HasValue)
                {
                    entityBase.PlanoContasId = contasAPagar.PlanoContasId.Value;
                }
                if (contasAPagar.CentroCustoId.HasValue)
                {
                    entityBase.CentroCustoId = contasAPagar.CentroCustoId.Value;
                }
                if (contasAPagar.ContaCorrenteId.HasValue)
                {
                    entityBase.ContaCorrenteId = contasAPagar.ContaCorrenteId.Value;
                }
                if (contasAPagar.ValorPago.HasValue)
                {
                    entityBase.ValorPago = contasAPagar.ValorPago.Value;
                }
                entityBase.SituacaoContaId = 2;
                entityBase.UpdateDate = DateTime.Now;
                entityBase.UpdateApplicationUserId = id;
                entityBase.DataPagamento = contasAPagar.DataPagamento;
                genericRepository.Update(entityBase);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no lançamento manual - " + ex);
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(ContasAPagar contasAPagar)
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
                if (contasAPagar.Id > decimal.Zero)
                {
                    var entityBase = genericRepository.Get(contasAPagar.Id);
                    entityBase.DataVencimento = contasAPagar.DataVencimento;
                    entityBase.ValorOriginal = contasAPagar.ValorOriginal;
                    entityBase.FornecedorId = contasAPagar.FornecedorId;
                    if (contasAPagar.CategoriaContasAPagarId.HasValue)
                    {
                        entityBase.CategoriaContasAPagarId = contasAPagar.CategoriaContasAPagarId.Value;
                    }
                    if (contasAPagar.PlanoContasId.HasValue)
                    {
                        entityBase.PlanoContasId = contasAPagar.PlanoContasId.Value;
                    }
                    if (contasAPagar.CentroCustoId.HasValue)
                    {
                        entityBase.CentroCustoId = contasAPagar.CentroCustoId.Value;
                    }
                    if (contasAPagar.ContaCorrenteId.HasValue)
                    {
                        entityBase.ContaCorrenteId = contasAPagar.ContaCorrenteId.Value;
                    }
                    entityBase.UpdateApplicationUserId = id;
                    entityBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(entityBase);
                }
                else
                {
                    contasAPagar.ApplicationUserId = id;
                    contasAPagar.CreateDate = DateTime.Now;
                    contasAPagar.EmpresaId = empresaId;
                    genericRepository.Insert(contasAPagar);
                }
                return new OkResult();

            }
            catch (Exception ex)
            {
                return BadRequest("Falha ao salvar a conta - " + ex.Message);
            }
        }

        [HttpPost()]
        [Route("relFornecedor")]
        [Authorize()]
        public IActionResult RelFornecedor(FilterDefault filter)
        {
            ClaimsPrincipal currentUser = this.User;
            var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
            var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
            if (id == null)
            {
                return BadRequest("Identificação do usuário não encontrada.");
            }
            Expression<Func<ContasAPagar, bool>> p1, p2, p3;
            var predicate = PredicateBuilder.New<ContasAPagar>();
            p1 = p => p.EmpresaId == empresaId;
            predicate = predicate.And(p1);
            p2 = p => p.SituacaoContaId == 1;
            predicate = predicate.And(p2);
            if (filter.FornecedorId > 0)
            {
                p3 = p => p.FornecedorId == filter.FornecedorId;
                predicate = predicate.And(p3);
            }
            var contas = contasaPagarRepository.Where(predicate).OrderByDescending(x => x.DataVencimento).ToList();
            if (contas.Count() == decimal.Zero)
            {
                object value = null;
                return Ok(value);
            }
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("relFornecedor");
                worksheet.SetShowGridLines(false);
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Relatório de contas a pagar por fornecedor";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.SetFontSize(14);
                worksheet.Range("A1:C3").Row(1).Merge();
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = string.Concat("Empresa: ", empresaRepository.Get(empresaId).Nome);
                worksheet.Range("A2:C3").Row(1).Merge();
                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Data de vencimento";
                worksheet.Cell(currentRow, 2).Value = "Fornecedor";
                worksheet.Cell(currentRow, 3).Value = "Categoria";
                worksheet.Cell(currentRow, 4).Value = "Centro de Custo";
                worksheet.Cell(currentRow, 5).Value = "Conta contábil";
                worksheet.Cell(currentRow, 6).Value = "Conta corrente";
                worksheet.Cell(currentRow, 7).Value = "Referente a";
                worksheet.Cell(currentRow, 8).Value = "Valor a pagar";
                IXLRange range = worksheet.Range(worksheet.Cell(currentRow, 1).Address, worksheet.Cell(currentRow, 8).Address);
                range.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                range.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                range.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                range.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Column("A").Width = 20;
                worksheet.Columns("B:G").Width = 30;
                worksheet.Column("H").Width = 20;

                currentRow = 5;
                contas.ForEach(conta =>
                {
                    worksheet.Cell(currentRow, 1).Value = conta.DataVencimento.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 2).Value = conta.Fornecedor.Nome;
                    if (conta.CategoriaContasAPagar != null)
                    {
                        worksheet.Cell(currentRow, 3).Value = conta.CategoriaContasAPagar.Nome;
                    }
                    if (conta.CentroCusto != null)
                    {
                        worksheet.Cell(currentRow, 4).Value = conta.CentroCusto.Descricao;
                    }
                    if (conta.PlanoContas != null)
                    {
                        worksheet.Cell(currentRow, 5).Value = conta.PlanoContas.Descricao;
                    }
                    if (conta.ContaCorrente != null)
                    {
                        worksheet.Cell(currentRow, 6).Value = string.Concat(conta.ContaCorrente.BancoNumero, " / ", conta.ContaCorrente.Banco, " / ", conta.ContaCorrente.Agencia, " / ", conta.ContaCorrente.Conta);
                    }
                    worksheet.Cell(currentRow, 7).Value = conta.Referente;
                    worksheet.Cell(currentRow, 8).Value = conta.ValorOriginal;
                    worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "#,###.00";
                    currentRow++;
                });
                worksheet.Cell(currentRow, 7).Value = "Total a pagar";
                worksheet.Cell(currentRow, 8).Value = contas.Sum(x => x.ValorOriginal);
                worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "#,###.00";
                worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 8).Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "relFornecedor.xlsx");
                }
            }
        }

        [HttpPost()]
        [Route("relProvisao")]
        [Authorize()]
        public IActionResult RelProvisao(FilterDefault filter)
        {
            ClaimsPrincipal currentUser = this.User;
            var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
            var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
            if (id == null)
            {
                return BadRequest("Identificação do usuário não encontrada.");
            }
            Expression<Func<ContasAPagar, bool>> p1, p2, p3;
            var predicate = PredicateBuilder.New<ContasAPagar>();
            p1 = p => p.EmpresaId == empresaId;
            predicate = predicate.And(p1);
            p2 = p => p.SituacaoContaId == 1;
            predicate = predicate.And(p2);
            p3 = p => p.DataVencimento.Month == filter.DataInicial.Month && p.DataVencimento.Year == filter.DataInicial.Year;
            predicate = predicate.And(p3);
            var contas = contasaPagarRepository.Where(predicate).OrderByDescending(x => x.DataVencimento).ToList();
            if (contas.Count() == decimal.Zero)
            {
                object value = null;
                return Ok(value);
            }
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("relProvisao");
                worksheet.SetShowGridLines(false);
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Provisão de contas a pagar";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.SetFontSize(14);
                worksheet.Range("A1:C3").Row(1).Merge();
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = string.Concat("Empresa: ", empresaRepository.Get(empresaId).Nome);
                worksheet.Range("A2:C3").Row(1).Merge();
                currentRow++;
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Data de vencimento";
                worksheet.Cell(currentRow, 2).Value = "Fornecedor";
                worksheet.Cell(currentRow, 3).Value = "Categoria";
                worksheet.Cell(currentRow, 4).Value = "Centro de Custo";
                worksheet.Cell(currentRow, 5).Value = "Conta contábil";
                worksheet.Cell(currentRow, 6).Value = "Conta corrente";
                worksheet.Cell(currentRow, 7).Value = "Referente a";
                worksheet.Cell(currentRow, 8).Value = "Valor a pagar";
                IXLRange range = worksheet.Range(worksheet.Cell(currentRow, 1).Address, worksheet.Cell(currentRow, 8).Address);
                range.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                range.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                range.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                range.Style.Fill.SetBackgroundColor(XLColor.LightGray);
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Column("A").Width = 20;
                worksheet.Columns("B:G").Width = 30;
                worksheet.Column("H").Width = 20;

                currentRow = 5;
                contas.ForEach(conta =>
                {
                    worksheet.Cell(currentRow, 1).Value = conta.DataVencimento.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 2).Value = conta.Fornecedor.Nome;
                    if (conta.CategoriaContasAPagar != null)
                    {
                        worksheet.Cell(currentRow, 3).Value = conta.CategoriaContasAPagar.Nome;
                    }
                    if (conta.CentroCusto != null)
                    {
                        worksheet.Cell(currentRow, 4).Value = conta.CentroCusto.Descricao;
                    }
                    if (conta.PlanoContas != null)
                    {
                        worksheet.Cell(currentRow, 5).Value = conta.PlanoContas.Descricao;
                    }
                    if (conta.ContaCorrente != null)
                    {
                        worksheet.Cell(currentRow, 6).Value = string.Concat(conta.ContaCorrente.BancoNumero, " / ", conta.ContaCorrente.Banco, " / ", conta.ContaCorrente.Agencia, " / ", conta.ContaCorrente.Conta);
                    }
                    worksheet.Cell(currentRow, 7).Value = conta.Referente;
                    worksheet.Cell(currentRow, 8).Value = conta.ValorOriginal;
                    worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "#,###.00";
                    currentRow++;
                });
                worksheet.Cell(currentRow, 7).Value = "Total a pagar";
                worksheet.Cell(currentRow, 8).Value = contas.Sum(x => x.ValorOriginal);
                worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "#,###.00";
                worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 8).Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "relFornecedor.xlsx");
                }
            }
        }

        [HttpPost()]
        [Route("exportDominio")]
        [Authorize()]
        public IActionResult ExportDominio(FilterDefault filter)
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
                if ((empresa.CodigoFilial == null) ||
                    (empresa.NomeUsuarioDominio == null) ||
                    (empresa.ContaTransitoria == null)) {
                    return BadRequest("Para gerar as partidas dobradas é necessário cadastrar Codigo da Filial no sistema Domínio, Nome do usuário no sistema Domínio e Conta transitória.");
                }

                Expression<Func<ContasAPagar, bool>> p1, p2, p3, p4, p6;
                Expression<Func<PlanoContas, bool>> p5;
                var predicate = PredicateBuilder.New<ContasAPagar>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                p2 = p => p.SituacaoContaId == 2;
                predicate = predicate.And(p2);
                p3 = p => p.DataPagamento.Value >= filter.DataInicial;
                predicate = predicate.And(p3);
                p4 = p => p.DataPagamento.Value <= filter.DataFinal;
                predicate = predicate.And(p4);
                p6 = p => p.PlanoContasId != null;
                predicate = predicate.And(p6);
                var pagamentos = contasaPagarRepository.Where(predicate).ToList();
                if (pagamentos.Count() == decimal.Zero)
                {
                    return BadRequest("Nenhum pagamento encontrado.");
                }
                var predicatePC = PredicateBuilder.New<PlanoContas>();
                p5 = p => p.EmpresaId == empresaId;
                predicatePC = predicatePC.And(p5);
                var planoContas = planoContasRepository.Where(predicatePC).ToList();
                if (planoContas.Count() == decimal.Zero)
                {
                    return BadRequest("Plano de contas não encontrado. Cadastro o plano de contas ou importe do sistema Domínio.");
                }
                MemoryStream memory = new MemoryStream();
                TextWriter textWriter = new StreamWriter(memory);
                string separador = ";";
                pagamentos.ForEach(pagamento =>
                {
                    textWriter.WriteLine(string.Concat(pagamento.DataPagamento.Value.ToString("dd/MM/yyyy"), separador, empresa.ContaTransitoria.Trim(), separador, planoContas.First(pl => pl.Id == pagamento.PlanoContasId).Classificacao.Replace(".", "").Trim(), separador, pagamento.ValorPago.Value.ToString().Replace(".",""), separador, pagamento.Id.ToString(), separador, pagamento.Referente, separador, separador, empresa.CodigoFilial.ToString(), separador, separador));
                    textWriter.WriteLine(string.Concat(pagamento.DataPagamento.Value.ToString("dd/MM/yyyy"), separador, planoContas.First(pl => pl.Id == pagamento.PlanoContasId).Classificacao.Replace(".", "").Trim(), separador, empresa.ContaTransitoria.Trim(), separador, pagamento.ValorPago.Value.ToString().Replace(".", ""), separador, pagamento.Id.ToString(), separador, pagamento.Referente, separador, separador, empresa.CodigoFilial.ToString(), separador, separador));
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
        [Route("lancamentonf")]
        [Authorize()]
        public async Task<IActionResult> LancamentoNF()
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                if ((id == null) || (empresaId == 0))
                {
                    return BadRequest("Acesso negado.");
                }

                var lst = new List<ContasAPagar>();
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Form.Files[0].CopyToAsync(memoryStream);
                    byte[] dataXML = memoryStream.ToArray();
                    XmlDocument oXML = new XmlDocument();
                    string xml = LocalEncoding.GetString(dataXML);
                    oXML.LoadXml(xml);
                    XmlNodeList nfeProc = oXML.GetElementsByTagName("nfeProc");
                    foreach (XmlElement element in nfeProc)
                    {

                    }

                        if (((XmlElement)nfeProc[0]).GetAttribute("versao") != "4.00")
                    {
                        return BadRequest("Versão incorreta do XML. Versão atual é a 4.00");
                    }

                    // Coleta a versão da NFE
                    XmlNodeList infNFe = oXML.GetElementsByTagName("infNFe");

                    foreach (XmlElement vr in infNFe)
                    {
                        //versaoNF = vr.Attributes["versao"].Value;
                    }

                    XmlNodeList emit = ((XmlElement)infNFe[0]).GetElementsByTagName("emit");
                    XmlNodeList dest = ((XmlElement)infNFe[0]).GetElementsByTagName("dest");
                    XmlNodeList ide = ((XmlElement)infNFe[0]).GetElementsByTagName("ide");
                    XmlNodeList vl = ((XmlElement)infNFe[0]).GetElementsByTagName("total");

                    //Loop para pegar os elementos CNPJ e Nome do Emitente
                    foreach (XmlElement nodo in emit)
                    {
                        XmlNodeList checaVazioEmit = nodo.GetElementsByTagName("CNPJ");

                        if (checaVazioEmit.Count == 1)
                        {
                            XmlNodeList CNPJ = nodo.GetElementsByTagName("CNPJ");
                            //CNPJCPFEmitente = Convert.ToUInt64(CNPJ[0].InnerText).ToString(@"00\.000\.000\/0000\-00");
                        }
                        else
                        {
                            XmlNodeList CNPJ = nodo.GetElementsByTagName("CPF");
                            //CNPJCPFEmitente = Convert.ToUInt64(CNPJ[0].InnerText).ToString(@"000\.000\.000\-00");
                        }

                        XmlNodeList Nome = nodo.GetElementsByTagName("xNome");
                        //NomeEmitente = Nome[0].InnerText;
                    }

                    //Loop para pegar os elementos CNPJ e Nome do Destinatario
                    foreach (XmlElement nodDest in dest)
                    {
                        //XmlNodeList checaVazio = nodDest.GetElementsByTagName("CNPJ");

                        //if (checaVazio.Count == 1)
                        //{
                        //    XmlNodeList CNPJ0 = nodDest.GetElementsByTagName("CNPJ");
                        //    CNPJCPFDestinatario = Convert.ToUInt64(CNPJ0[0].InnerText).ToString(@"00\.000\.000\/0000\-00");
                        //}
                        //else
                        //{
                        //    XmlNodeList CNPJ0 = nodDest.GetElementsByTagName("CPF");
                        //    CNPJCPFDestinatario = Convert.ToUInt64(CNPJ0[0].InnerText).ToString(@"000\.000\.000\-00");
                        //}

                        //XmlNodeList Nome0 = nodDest.GetElementsByTagName("xNome");
                        //NomeDestinatario = Nome0[0].InnerText;
                    }

                    //Loop para pegar os elementos Natureza, Numero NF e Data de emissão
                    foreach (XmlElement nodIde in ide)
                    {
                        //XmlNodeList nat = nodIde.GetElementsByTagName("natOp");
                        //XmlNodeList num = nodIde.GetElementsByTagName("nNF");

                        //if (versaoNF == "3.10")
                        //{
                        //    XmlNodeList dtEmissao = nodIde.GetElementsByTagName("dhEmi");
                        //    dataEmissao = dtEmissao[0].InnerText;
                        //}

                        //if (versaoNF == "2.00")
                        //{
                        //    XmlNodeList dtEmissao = nodIde.GetElementsByTagName("dEmi");
                        //    dataEmissao = dtEmissao[0].InnerText;
                        //}

                        //XmlNodeList tipo = nodIde.GetElementsByTagName("tpNF");

                        //natOperacao = nat[0].InnerText;
                        //numeroNF = num[0].InnerText;
                        //tipoNF = tipo[0].InnerText;
                    }

                    //Loop para pegar os elementos Identificação da Nota
                    foreach (XmlNode oNo in infNFe)
                    {
                        //chaveNFE = oNo.Attributes.GetNamedItem("Id").Value;
                        //chaveNFE = chaveNFE.Replace("NFe", "");
                    }

                    //nfe.arquivoXML = dataXML;
                    //nfe.nomeXML = attachment.Filename;
                }

                return new JsonResult(lst);
            }
            catch (Exception ex)
            {
                return BadRequest("Falha na conversão do arquivo - " + ex.Message);
            }

        }
    }
}
