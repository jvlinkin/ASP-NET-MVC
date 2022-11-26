using ControleDeContatos.Models;
using ControleDeContatos.Repositório;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleDeContatos.Controllers
{

    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }
        public IActionResult Index()
        {
            var usuarios = _usuarioRepositorio.BuscarTodos();
            return View(usuarios);
        }

        public IActionResult Criar()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Criar(UsuarioModel usuario) //Quem irá fazer o trabalho de inserir no banco de dados, é o método do repositório   
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                _usuarioRepositorio.Adicionar(usuario);
                TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
                return RedirectToAction("Index");

            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos cadastrar seu usuário. Tente novamente. Erro: {erro.Message}";
                return RedirectToAction("Index");
            }

        }

        public IActionResult Editar(int id)
        {
            UsuarioModel usuario = _usuarioRepositorio.BuscarPorId(id);
            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioSemSenhaModel usuarioSemSenhaModel)
        {
            try
            {
                UsuarioModel usuario = null;
                if (ModelState.IsValid)
                {
                    usuario = new UsuarioModel()
                    {
                        Id = usuarioSemSenhaModel.Id,
                        Nome = usuarioSemSenhaModel.Nome,
                        Login = usuarioSemSenhaModel.Login,
                        Email = usuarioSemSenhaModel.Email,
                        Perfil = usuarioSemSenhaModel.Perfil
                    };


                    usuario = _usuarioRepositorio.Atualizar(usuario);
                    TempData["MensagemSucesso"] = "Usuário atualizado com sucesso.";
                    return RedirectToAction("Index");
                }
               
                return View(usuario);

            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Houve um erro ao atualizar o usuáro. Tente novamente. Erro:{erro}";
                return RedirectToAction("Index");
            }
        }


        public IActionResult ApagarConfirmacao(int id)
        {
            var user = _usuarioRepositorio.BuscarPorId(id);
            if(user == null)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos encontrar o usuário especificado. Tente novamente.";
                return View("Index");
            }
            return View(user);

        }

        public IActionResult Apagar(int id)
        {
            try
            {
                _usuarioRepositorio.Apagar(id);
                TempData["MensagemSucesso"] = "Usuário deletado com sucesso.";
                return RedirectToAction("Index");

            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Houve um erro ao deletar o usuário. Erro:{erro}";
                return RedirectToAction("Index");

            }



        }
    }
}
