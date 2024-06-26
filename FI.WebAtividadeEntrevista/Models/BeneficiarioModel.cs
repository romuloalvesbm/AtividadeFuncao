﻿using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long? Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public long IdCliente { get; set; }
        public ClienteModel ClienteModel { get; set; }
    }
}
