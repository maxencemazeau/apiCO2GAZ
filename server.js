const express = require('express');
const cors = require('cors');
const app = express();
const bodyParser = require('body-parser');
require('dotenv').config();
const port = process.env.PORT || 8080;
const faunadb = require('faunadb');
const q = faunadb.query;

const client = new faunadb.Client({
  secret: process.env.FAUNADB_SECRET,
});

app.listen(port, () => console.log('Listen on port ' + port));

const corsOptions = {
  origin: '*',
  method: 'GET,PUT,POST,DELETE,OPTIONS',
  allowedHeaders: ['Content-Type'],
  credentials: true,
  optionSuccessStatus: 200,
};

app.use(cors(corsOptions));
app.use(express.json());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));

async function getAllUsers() {
    try {
      const result = await client.query(
        q.Map(
          q.Paginate(q.Documents(q.Collection('user'))),
          q.Lambda('userRef', q.Get(q.Var('userRef')))
        )
      );
      return result.data;
    } catch (error) {
      console.error('Error fetching users:', error);
      throw error;
    }
  }
  
  // Add a new API route to get all users
  app.get('/api/controller/users', async (req, res) => {
    try {
      const users = await getAllUsers();
      res.json(users);
    } catch (error) {
      console.error('Error fetching users:', error);
      res.status(500).send('Internal server error');
    }
  });

// Add your updated API routes here
app.post('/api/controller/connexion', async function (req, res) {
    const { login, password } = req.body;
  
    try {
      const users = await client.query(
        q.Map(
          q.Paginate(q.Documents(q.Collection('user'))),
          q.Lambda('userRef', q.Get(q.Var('userRef')))
        )
      );
  
      const user = users.data.find(
        (user) => user.data.login === login && user.data.password === password
      );
  
      if (user) {
        console.log('success');
        res.send({
          id: user.ref.id,
          login: user.data.login,
          password: user.data.password,
        });
      } else {
        res.status(401).send('Invalid username or password');
      }
    } catch (err) {
      console.log(err);
      res.status(500).send('Internal server error');
    }
  });
  
  