using ControleDeContatos.Filters;
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
    [PaginaParaUsuarioLogado]
    public class ContatoController : Controller
    {   
        //Injeção de dependência
        private readonly IContatoRepositório _contatoRepositorio;
        private readonly ISessao _sessao;
        public ContatoController(IContatoRepositório contatoRepositório,
                                 ISessao sessao)
        {
            _contatoRepositorio = contatoRepositório;
            _sessao = sessao;
        }

        public IActionResult Index()
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            var contatos = _contatoRepositorio.BuscarTodos(usuarioLogado.Id);
            return View(contatos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            ContatoModel contato = _contatoRepositorio.BuscarPorId(id);
            return View(contato);
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            ContatoModel contato = _contatoRepositorio.BuscarPorId(id);
            return View(contato);
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                _contatoRepositorio.Apagar(id);
                TempData["MensagemSucesso"] = "Contato deletado com sucesso.";
                return RedirectToAction("Index");

            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Houve um erro ao deletar o contato. Erro:{erro}";
                return RedirectToAction("Index");

            }

            
            
        }

        [HttpPost]
        public ActionResult Criar(ContatoModel contato) //Quem irá fazer o trabalho de inserir no banco de dados, é o método do repositório   
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contato);
                }

                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                contato.UsuarioId = usuarioLogado.Id;
                _contatoRepositorio.Adicionar(contato);
                TempData["MensagemSucesso"] = "Contato cadastrado com sucesso";
                return RedirectToAction("Index");

            }
            catch(System.Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos cadastrar seu contato. Tente novamente. Erro: {erro.Message}";
                return RedirectToAction("Index");
            }
            
            

        }

        [HttpPost]
        public ActionResult Alterar(ContatoModel contato)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Editar", contato);
                }

                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                contato.Id = usuarioLogado.Id;
                _contatoRepositorio.Atualizar(contato);
                TempData["MensagemSucesso"] = "Cadastro atualizado com sucesso.";
                return RedirectToAction("Index");


            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Houve um erro ao atualizar o contato. Tente novamente. Erro:{erro}";
                return RedirectToAction("Index");
            }

            

        }

    }
}
