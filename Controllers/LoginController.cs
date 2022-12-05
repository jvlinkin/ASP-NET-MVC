using ControleDeContatos.Helper;
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
        private readonly ISessao _sessao;
        private readonly IEmail _email;
        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                               ISessao sessao, IEmail iemail)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _email = iemail;
        }
        public IActionResult Index()
        {
            //Se o usuário estiver logado, redirecionar para a Home
            if(_sessao.BuscarSessaoDoUsuario() != null)
            {
                return RedirectToAction("Index", "Home");
            }
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
                            _sessao.CriarSessaoDoUsuario(user);
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
        
       [HttpPost]
        public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {

            try
            {
                if (ModelState.IsValid)
                
                {
                    UsuarioModel user = _usuarioRepositorio.BuscarPorEmailELogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);
                    

                    if (user != null)

                    {
                        string novaSenha = user.GerarNovaSenha();
                        string mensagem = $"Sua nova senha é: {novaSenha}";
                        bool emailEnviado = _email.Enviar(user.Email, "Sistema de Contatos - Nova Senha", mensagem);

                        if (emailEnviado)
                        {
                            _usuarioRepositorio.Atualizar(user);
                            TempData["MensagemSucesso"] = $"Enviamos para seu e-mail cadastrado, uma nova senha.";
                        }
                        else
                        {
                            TempData["MensagemErro"] = $" Não foi possível enviar o email. Tente novamente";
                        }

                        
                        return RedirectToAction("Index", "Login");

                    }

                    TempData["MensagemErro"] = "Não conseguimos redefinir sua senha. Verifique os dados informados.";


                }

                return View("Index");


            }
            catch (Exception erro)
            {

                TempData["MensagemErro"] = $"Ops, não conseguimos redefinir sua senha. Tente novamente. {erro.Message}";
                return RedirectToAction("Index");
            }

        }


        public IActionResult RedefinirSenha()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Index", "Login");
        }
    }
}
