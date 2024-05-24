using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Web.Helpers;
using WebAtividadeEntrevista.Util;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var cpf = model.Cpf.Replace(".", "").Replace("-", "");                

                //Validações
                if (!Valida.Cpf(cpf)) 
                {
                    return Json(new
                    {
                        success = false,
                        message = "CPF inválido"
                    });
                }

                if (bo.VerificarExistencia(cpf))
                {
                    return Json(new
                    {
                        success = false,
                        message = "CPF já cadastrado."
                    });
                }

                model.Id = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    Cpf = cpf,
                    Beneficiarios = model.Beneficiarios != null && model.Beneficiarios.Any() ? model.Beneficiarios.Select(x => new Beneficiarios() { Cpf = x.Cpf, Nome = x.Nome}).ToList() : new List<Beneficiarios>()
                });

           
                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var cpf = model.Cpf.Replace(".", "").Replace("-", "");

                var cliente = bo.Consultar(model.Id);

                if (cliente == null)
                {                    
                    return Json(new
                    {
                        success = false,
                        message = "Operação cancelada, cliente não encontrado."
                    });
                }
                else
                {
                    var clienteAux = bo.ConsultarPorCpf(cpf);

                    if (clienteAux != null && (cliente.Id != clienteAux.Id && cpf == clienteAux.Cpf))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "CPF já cadastrado com outro cliente."
                        });
                    }                      

                    if (cliente.Cpf != cpf)
                    {
                        //Validações
                        if (!Valida.Cpf(cpf))
                        {                           
                            return Json(new
                            {
                                success = false,
                                message = "CPF inválido."
                            });
                        }
                    }
                }

                var listBeneficiarios = new List<Beneficiarios>();

                if (model.Beneficiarios.Any()) 
                {                    
                    foreach (var item in model.Beneficiarios)
                    {
                        //Novos Beneficiarios
                        if(item.Id == null && item.Cpf != null) 
                        {
                            listBeneficiarios.Add(new Beneficiarios
                            {
                                Cpf = item.Cpf,
                                Nome = item.Nome
                            });
                        }
                    }
                }

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    Cpf = cpf,
                    Beneficiarios = listBeneficiarios.Any() ? listBeneficiarios : null
                });
                               
                return Json("Cadastro alterado com sucesso.");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                var aa = Convert.ToUInt64(cliente.Cpf);
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    Cpf = Convert.ToUInt64(cliente.Cpf).ToString(@"000\.000\.000\-00"),
                    Beneficiarios = cliente.Beneficiarios.Select(x => new BeneficiarioModel() { Id = x.Id, Cpf = x.Cpf, Nome = x.Nome }).ToList()
                };            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}