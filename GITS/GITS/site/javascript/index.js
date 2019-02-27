var user = new Object();

$(document).ready(function(){
  $('#slide-out').sidenav({
    edge: 'right'
  });
  $('#slide-out-batata').sidenav();
  $(".dropdown-trigger").dropdown();
});



if (user)
{
    var body = document.getElementsByTagName('body')[0];
    var cod = `
    
      <ul id="slide-out-batata" class="sidenav" id="triggerEsquerda">
        <img src="https://static1.conquistesuavida.com.br/ingredients/5/54/52/05/@/24682--ingredient_detail_ingredient-2.png">
      </ul>
      <img src="https://upload.wikimedia.org/wikipedia/commons/7/7a/Crunchyroll_logo_2012v.png" data-target="slide-out-batata" class="sidenav-trigger" id="pfFunciona">`;


    $("nav").after(cod);
}
else
{
    console.log(';-;')
}
function onSignIn(googleUser) {
  var profile = googleUser.getBasicProfile();
  console.log(profile)
  document.getElementById('pfFunciona').src = profile.Paa;
}