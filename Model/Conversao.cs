
using System;
using System.Collections.Generic;

namespace Model
{
    public class Conversao
    { 
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<PartidaDobrada> Partidas { get; set; }
    }
}
