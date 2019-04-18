using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;

namespace GITS.ViewModel
{
    public class Usuario
    {
        public string CodUsuario { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string FotoPerfil { get; set; }
        public Usuario[] Amigos { get; set; }
        public int XP { get; set; }
        public int XpTotal { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        public int Insignia { get; set; }
        public double Dinheiro { get; set; }
        public string Titulo { get; set; }
        public int TemaSite { get; set; }
        public int Decoracao { get; set; }


        internal static Usuario GetLoginInfo(ClaimsIdentity identity)
        {
            if (identity.Claims.Count() == 0 || identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email) == null)
            {
                return null;
            }
            var ret = new Usuario();

            ret.CodUsuario = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var accessToken = identity.Claims.Where(c => c.Type.Equals("urn:google:accesstoken")).Select(c => c.Value).FirstOrDefault();
            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + accessToken);
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(apiRequestUri);
                dynamic result = JsonConvert.DeserializeObject(json);
                ret.FotoPerfil = result.picture;
            }
            ret.Email = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            ret.Nome = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            ret.Amigos = new Usuario[0];
            ret.XP = 3000;
            ret.XpTotal = 100;
            ret.Level = 1;
            ret.Status = "Muito Bom Dia";
            ret.Insignia = 5;
            ret.Dinheiro = 4032.45;
            ret.Titulo = "Nao sei";
            ret.TemaSite = 2;
            //pegar do banco o usuario com esse id, se nao tiver, criar um
            //e verificar/atualizar tudo
            //e pegar info
            return ret;
        }
    }
}