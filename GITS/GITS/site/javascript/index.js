var user = new Object();

$(document).ready(function(){
  $('#slide-out').sidenav({
    edge: 'right'
  });
  $('#slide-out-batata').sidenav();
  $(".dropdown-trigger").dropdown({
    constrainWidth: false
  });
});

var profile = {};
var startApp = function() {
  gapi.load('auth2', function(){
    // Retrieve the singleton for the GoogleAuth library and set up the client.
    auth2 = gapi.auth2.init({
      client_id: '766728087059-hviu4rg9tr8quuv86v1ldh72t1a7poso.apps.googleusercontent.com',
      cookiepolicy: 'single_host_origin',
      // Request scopes in addition to 'profile' and 'email'
      //scope: 'additional_scope'
    });
    console.log(auth2)
  });
};
function signOut() {
  var auth2 = gapi.auth2.getAuthInstance();
  auth2.signOut();
  window.location.reload();
}

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
function config(profile) {
  document.getElementById('loginGoogle').innerHTML = 'Logout';
  document.getElementById('loginGoogle').onclick = signOut;
  console.log(profile)
  document.getElementById('pfFunciona').src = profile.Paa.substring(0, profile.Paa.length - 10);
  user = profile;
}

setTimeout(startApp, 50)


/*
auth2.attachClickHandler(document.getElementById('loginGoogle'), {},
      function(googleUser) {
        config(googleUser.getBasicProfile());
      }, function(error) {
        //alert(JSON.stringify(error, undefined, 2));
      });
      */  