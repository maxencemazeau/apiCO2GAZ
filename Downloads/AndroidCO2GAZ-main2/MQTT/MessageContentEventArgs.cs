/* Copyright (C) 2023 Maxence MAZEAU
 * All rights reserved.
 *
 * Projet Qualite de l'air
 * Ecole du Web
 * Projet technologique (c)2023
 *  

    Historique des versions
           Version    Date       Auteur       Description
           1.1        24/02/23  Maxence     Première version
           1.2         23/04/23  Maxence     Deuxième version
           1.3       30/04/23   Maxence     Troisième version
   
 * */

using System;

namespace MQTT
{
    public class MessageContentEventArgs : EventArgs
    {
        public string MessageContent { get; set; }

        public MessageContentEventArgs(string messageContent)
        {
            MessageContent = messageContent;
        }
    }
}
