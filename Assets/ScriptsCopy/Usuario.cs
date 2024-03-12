using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Usuario
{
    /*
        "_id": "6542d1d6a60acfd5a6b802bf",
        "nome": "Pedro Carvalho",
        "nick": "PedrinCar",
        "email": "carvalho_pedro@email.com",
        "senha": "$2b$10$mY0jLvA4lqWow/nvlcoM/eRtPveQ5nrvKaHKmzppf7ut1gGdWhftW",
        "codValida": null,
        "__v": 0
     */
    public string _id;
    public string nome;
    public string nick;
    public string email;
    public string senha;
    public string codValida;
    public int __v;
}
