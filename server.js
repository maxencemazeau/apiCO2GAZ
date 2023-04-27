const express = require('express');
// const mysql = require('mysql');
const cors = require ('cors');
const app = express();
const bodyParser = require('body-parser');
const port = process.env.PORT || 8080;
require('dotenv').config()
const mysql = require('mysql2')
//const connection = mysql.createConnection(process.env.DATABASE_URL)
console.log('Connected to PlanetScale!')



app.listen(port, () => console.log('Listen on port ' + port))
// //Mysql

app.use(function(req, res, next){
    connection = mysql.createConnection({
    connectionLimit : 10,
    database: 'co2gaz',
    username: 'eip30hk3zz3liqpv2smw',
    host: 'gcp.connect.psdb.cloud',
    password: '************'

});
    res.locals.connection.connect();
    next();
});

const corsOptions ={
    origin:'*', 
    method : 'GET,PUT,POST,DELETE,OPTIONS',
    allowedHeaders: ['Content-Type'],
    credentials:true,            //access-control-allow-credentials:true
    optionSuccessStatus:200,
    
}
app.use(cors(corsOptions));


// JSON body parser, there is no `extended` option here
app.use(express.json());

// parse application/json
app.use(bodyParser.json())

// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: false }))


//API Utilisateur 
app.post('/api/controller/connexion', function(req, res){
    const { login, password } = req.body;
    // SQL query to check if user exists and password is correct
    const query = `SELECT * FROM utilisateur WHERE login = '${login}' AND password = '${password}'`;
    console.log(query);
    // Execute the SQL query
    connection.query(query, function(err, rows) {
      if (err) {
        console.log(err);
        res.status(500).send('Internal server error');
      } else if (rows.length == 0) {
        res.status(401).send('Invalid username or password');
      } else {
        const utilisateur = rows[0];
        console.log('success');
        // Successful login, send back the user data
        res.send({
          id: utilisateur.id,
          login: utilisateur.login,
          });
      }
    });
  });

  app.get('/api/controller/utilisateur', (req, res) => {
    connection('SELECT * from utilisateur', function(req, res){
        if(error) throw error;
        res.json(results);
    })
  })

//API pour gérer le GAZ et CO2

app.get('/api/controller/historiqueCO2', function(req, res, next){ 
   res.locals.connection.query('Select * from historiqueCO2', function(error, results, fields){
        if (error) throw error;
        res.json(results);
    })
});

app.get('/api/controller/historiqueGAZ', function(req, res, next){ 
    res.locals.connection.query('Select * from historiqueGAZ', function(error, results, fields){
        if (error) throw error;
        res.json(results);
    })
});

app.post('/api/controller/envoieCO2', (req, res) => {
    console.log(req.body);
    const  {niveau} = req.body;
    const date = new Date();
    const utilisateurId = 1;
    const sql = `INSERT INTO historiqueCO2 (niveau, date, utilisateurId) values (?,?,?)`;
    res.locals.connection.query(sql, [niveau, date, utilisateurId], (error, results) => {
        if (error){
            console.log(error);
        } else {
            res.send('Donnée insérées');
        }
    })
});

app.post('/api/controller/envoieGAZ', (req, res) => {
    console.log(req.body);
    const  {niveau} = req.body;
    const date = new Date();
    const utilisateurId = 1;
    const sql = `INSERT INTO historiqueGAZ (niveau, date, utilisateurId) values (?,?,?)`;
    res.locals.connection.query(sql, [niveau, date, utilisateurId], (error, results) => {
        if (error){
            console.log(error);
        } else {
            res.send('Donnée insérées');
        }
    })
});