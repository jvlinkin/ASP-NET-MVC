using ControleDeContatos.Models;
using ControleDeContatos.Repositório;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleDeContatos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        public LoginController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;     
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Entrar(LoginModel loginModel)
        {
            try
            {
                //Verificando se os campos da model estão preenchidos. 
                
                if (ModelState.IsValid)
                    /*Se entra no if, os dados que o usuário digitou ficam acessíveis dentro de
                    loginModel.NomeDoAtributo.*/
                {
                    UsuarioModel user = _usuarioRepositorio.BuscarPorLogin(loginModel.Login);
                    /*Nesse momento, caso ele ache, todos os dados da model, estarão disponíveis dentro
                    da variável user*/

                    if(user != null)
                    {
                        //if(user.Senha == loginModel.Senha)
                        if (user.SenhaValida(loginModel.Senha))                            
                        {
                            //realiza login
                            return RedirectToAction("Index", "Home");
                        }                        
                        
                    }
                    TempData["MensagemErro"] = "Usuário e/ou senha inválidos. Tente novamente";
                    
                }

                return View("Index");


            }
            catch (Exception erro)
            {

                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login. Tente novamente. {erro.Message}";
                return RedirectToAction("Index");
            }

        }
    }
}
