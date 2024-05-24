using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Cliente
    /// </summary>
    internal class DaoCliente : AcessoDados
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal long Incluir(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();
            
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", cliente.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CEP", cliente.CEP));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Estado", cliente.Estado));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Email", cliente.Email));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cpf", cliente.Cpf));

            if (cliente.Beneficiarios != null)
            {
                // Criar a tabela de beneficiários
                DataTable beneficiariosTable = new DataTable();
                beneficiariosTable.Columns.Add("Nome", typeof(string));
                beneficiariosTable.Columns.Add("CPF", typeof(string));

                // Preencher a tabela de beneficiários com os dados dos beneficiários do cliente
                foreach (var beneficiario in cliente.Beneficiarios)
                {
                    beneficiariosTable.Rows.Add(beneficiario.Nome, beneficiario.Cpf);
                }

                // Adicionar o parâmetro para os beneficiários
                SqlParameter beneficiariosParam = new SqlParameter("@BENEFICIARIOS", beneficiariosTable);
                beneficiariosParam.SqlDbType = SqlDbType.Structured;
                beneficiariosParam.TypeName = "BeneficiariosType";
                parametros.Add(beneficiariosParam);
            }

            DataSet ds = base.Consultar("FI_SP_IncClienteV2", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal DML.Cliente Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli.FirstOrDefault();
        }

        internal bool VerificarExistencia(string CPF)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));

            DataSet ds = base.Consultar("FI_SP_VerificaCliente", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        internal DML.Cliente ConsultarPorCpf(string CPF)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));

            DataSet ds = base.Consultar("FI_SP_VerificaClientePorCpf", parametros);

            List<DML.Cliente> cli = Converter(ds);

            return cli.FirstOrDefault();
        }

        internal List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("iniciarEm", iniciarEm));
            parametros.Add(new System.Data.SqlClient.SqlParameter("quantidade", quantidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("campoOrdenacao", campoOrdenacao));
            parametros.Add(new System.Data.SqlClient.SqlParameter("crescente", crescente));

            DataSet ds = base.Consultar("FI_SP_PesqCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            int iQtd = 0;

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out iQtd);

            qtd = iQtd;

            return cli;
        }

        /// <summary>
        /// Lista todos os clientes
        /// </summary>
        internal List<DML.Cliente> Listar()
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", 0));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Alterar(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", cliente.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CEP", cliente.CEP));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Estado", cliente.Estado));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Email", cliente.Email));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", cliente.Id));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", cliente.Cpf));

            if (cliente.Beneficiarios != null)
            {
                // Criar a tabela de beneficiários
                DataTable beneficiariosTable = new DataTable();
                beneficiariosTable.Columns.Add("Nome", typeof(string));
                beneficiariosTable.Columns.Add("CPF", typeof(string));

                // Preencher a tabela de beneficiários com os dados dos beneficiários do cliente
                foreach (var beneficiario in cliente.Beneficiarios)
                {
                    beneficiariosTable.Rows.Add(beneficiario.Nome, beneficiario.Cpf);
                }

                // Adicionar o parâmetro para os beneficiários
                SqlParameter beneficiariosParam = new SqlParameter("@BENEFICIARIOS", beneficiariosTable);
                beneficiariosParam.SqlDbType = SqlDbType.Structured;
                beneficiariosParam.TypeName = "BeneficiariosType";
                parametros.Add(beneficiariosParam);
            }

            base.Executar("FI_SP_AltCliente", parametros);
        }


        /// <summary>
        /// Excluir Cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelCliente", parametros);
        }

        private List<DML.Cliente> Converter(DataSet ds)
        {
            List<DML.Cliente> lista = new List<DML.Cliente>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    long id = row.Field<long>("Id");
                    DML.Cliente cliente = lista.FirstOrDefault(c => c.Id == id);

                    if (cliente == null)
                    {
                        cliente = new DML.Cliente();
                        cliente.Id = row.Field<long>("Id");
                        cliente.CEP = row.Field<string>("CEP");
                        cliente.Cidade = row.Field<string>("Cidade");
                        cliente.Email = row.Field<string>("Email");
                        cliente.Estado = row.Field<string>("Estado");
                        cliente.Logradouro = row.Field<string>("Logradouro");
                        cliente.Nacionalidade = row.Field<string>("Nacionalidade");
                        cliente.Nome = row.Field<string>("Nome");
                        cliente.Sobrenome = row.Field<string>("Sobrenome");
                        cliente.Telefone = row.Field<string>("Telefone");
                        cliente.Cpf = row.Field<string>("Cpf");
                        cliente.Beneficiarios = new List<Beneficiarios>();

                        lista.Add(cliente);
                    }

                    if (row != null && row.Table.Columns.Contains("BeneficiarioId") && !row.IsNull("BeneficiarioId"))
                    {
                        var beneficiario = new Beneficiarios
                        {
                            Id = row.Field<long>("BeneficiarioId"),
                            Cpf = row.Field<string>("BeneficiarioCpf"),
                            Nome = row.Field<string>("BeneficiarioNome"),
                            IdCliente = row.Field<long>("Id")
                        };
                        cliente.Beneficiarios.Add(beneficiario);
                    }                    
                        
                }
            }

            return lista;
        }
    }
}
