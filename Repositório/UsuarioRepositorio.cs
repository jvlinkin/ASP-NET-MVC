using ControleDeContatos.Data;
using ControleDeContatos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleDeContatos.Repositório
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly BancoContext _bancoContext;
        public UsuarioRepositorio(BancoContext bancoContext) //Acima jogamos o "bancoContext" para uma variável, para conseguirmos usar os métodos
        {
            _bancoContext = bancoContext;

        }

        public UsuarioModel BuscarPorLogin(string login)
        {
            return _bancoContext.Usuarios.FirstOrDefault(x => x.Login.ToUpper() == login.ToUpper());
        }

        public UsuarioModel BuscarPorEmailELogin(string email, string login)
        {
            return _bancoContext.Usuarios.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper() && x.Login.ToUpper() == login.ToUpper());
        }

        public UsuarioModel BuscarPorId(int id)
        {
            return _bancoContext.Usuarios.FirstOrDefault(x => x.Id == id);
        }


        public List<UsuarioModel> BuscarTodos()
        {
            return _bancoContext.Usuarios.ToList();
        }
        public UsuarioModel Adicionar(UsuarioModel usuario)
        {
            usuario.DataDeCadastro = DateTime.Now;
            usuario.SetSenhaHash();
            _bancoContext.Usuarios.Add(usuario);
            _bancoContext.SaveChanges();

            return usuario;
        }

        public UsuarioModel Atualizar(UsuarioModel usuario)
        {
            var user = BuscarPorId(usuario.Id);

            if (user == null)
            {
                throw new System.Exception("Ocorreu um erro ao atualizar o usuário.");
            }

            user.Nome = usuario.Nome;
            user.Email = usuario.Email;
            user.Login = usuario.Login;
            user.Perfil = usuario.Perfil;
            user.DataAtualizacao = DateTime.Now;
            

            _bancoContext.Usuarios.Update(user);
            _bancoContext.SaveChanges();

            return user;
        }

        public bool Apagar(int id)
        {

            UsuarioModel user = BuscarPorId(id);
            if(user == null)
            {
                throw new System.Exception("Ocorreu um erro ao excluir o usuário.");
            }

            _bancoContext.Usuarios.Remove(user);
            _bancoContext.SaveChanges();

            return true;
        }

        public UsuarioModel AlterarSenha(AlterarSenhaModel alterarSenhaModel)
        {
            UsuarioModel user = BuscarPorId(alterarSenhaModel.Id);

            if(user == null)
            {
                throw new Exception("Houve um erro na atualização da senha. Usuário não encontrado.");
            }
            /*O método SenhaValida, ele compara a senha digitada com a senha atual do usuário no BD. Se válida, ele retorna true,
            caso não, ele retorna false.*/
            if (!user.SenhaValida(alterarSenhaModel.SenhaAtual))
            {
                throw new Exception("Senha atual não confere.");
            }

            /*Validando se a senha que o usuário quer utilizar, já não é a mesma do banco.*/
            if (user.SenhaValida(alterarSenhaModel.NovaSenha)){
                throw new Exception("Nova senha deve ser diferente da senha atual.");
            }

            user.SetNovaSenha(alterarSenhaModel.NovaSenha);
            user.DataAtualizacao = DateTime.Now;

            _bancoContext.Usuarios.Update(user);
            _bancoContext.SaveChanges();
            return user;







        }
    }
}
