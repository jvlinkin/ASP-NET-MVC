using ControleDeContatos.Data;
using ControleDeContatos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleDeContatos.Repositório
{
    public class ContatoRepositório : IContatoRepositório
    {
        private readonly BancoContext _bancoContext;
        public ContatoRepositório(BancoContext bancoContext) //Acima jogamos o "bancoContext" para uma variável, para conseguirmos usar os métodos
        {
            _bancoContext = bancoContext;

        }

        public ContatoModel BuscarPorId(int id)
        {
            return _bancoContext.Contatos.FirstOrDefault(x => x.Id == id);
        }


        public List<ContatoModel> BuscarTodos(int usuarioId)
        {
            return _bancoContext.Contatos.Where(x=>x.UsuarioId == usuarioId).ToList();
        }
        public ContatoModel Adicionar(ContatoModel contato)
        {
            _bancoContext.Contatos.Add(contato);
            _bancoContext.SaveChanges();

            return contato;
        }

        public ContatoModel Atualizar(ContatoModel contato)
        {
            var user = BuscarPorId(contato.Id);

            if (user == null)
            {
                throw new System.Exception("Ocorreu um erro.");
            }

            user.Nome = contato.Nome;
            user.Email = contato.Email;
            user.Celular = contato.Celular;

            _bancoContext.Contatos.Update(user);
            _bancoContext.SaveChanges();

            return user;
        }

        public bool Apagar(int id)
        {

            var user = BuscarPorId(id);
            if(user == null)
            {
                throw new System.Exception("Ocorreu um erro ao excluir o usuário.");
            }

            _bancoContext.Contatos.Remove(user);
            _bancoContext.SaveChanges();

            return true;
        }
    }
}
