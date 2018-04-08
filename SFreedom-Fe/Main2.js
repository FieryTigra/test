


window.onload = function() { 


   var strGET = window.location.search.replace( '?', ''); 
   var params = window
    .location
    .search
    .replace('?','')
    .split('&')
    .reduce(
        function(p,e){
            var a = e.split('=');
            p[ decodeURIComponent(a[0])] = decodeURIComponent(a[1]);
            return p;
        },
        {}
    );

 if(params['address'] != undefine){
    getTweet(params['address']);
 };
// выведет в консоль значение  GET-параметра data
}






var app = new Vue (
{
  el: '#well123',
  data: 
  {
     postMass : [],
     msg : ""
  },
  created: function () 
  {
      
  },
  methods:
  {
    
  }
});
var form = new Vue (
{
  el: '#form123',
  data: 
  {

     address : ""
  },
  created: function () 
  {
      
  },
  methods:
  {
    
  }
});

function onPostSend(){
    var xhr = new XMLHttpRequest();
   console.log("post");
      xhr.open('POST', 'http://localhost:1549/ext/NodeAPI/addPost', false);
      xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

// 3. Отсылаем запрос
xhr.send("msg="+form.msg);

}
function getPost () {
  var xhr = new XMLHttpRequest();
      console.log("post");
      xhr.open('POST', 'http://localhost:1549/ext/NodeAPI/getTweet', false);
      xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

// 3. Отсылаем запрос
xhr.send("address="+form.address);
  console.log(xhr.response);
  var objectqwe = {};
 objectqwe.data = "";
 objectqwe.isOk = true;
  objectqwe = JSON.parse(xhr.response)
 app.postMass = objectqwe.data;

    }

function add(msg, like){
  var object = {};
  object.msg = msg;
  object.like = like;
  app.postMass.push(object);
}