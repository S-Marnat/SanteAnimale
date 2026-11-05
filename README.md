# **Santé Animale**

**Santé Animale** est une application visant à aider les propriétaires à gérer la santé de leurs animaux de compagnie.

**L’application est actuellement en cours de développement.**

La solution est créée grâce à ASP.NET Core 10.0, et utilise la structure en microservice.
La solution comprendra :
  - 7 projets Back-End de type API ;
  - 1 projet Front-End (ou un front par fonctionnalité, au besoin) ;
  - 1 projet Back-End de type Gateway ;
  - 7 projets de tests (un pour chaque projet de type API).

Les suppressions en cascade se dérouleront dans un ordre bien précis :
  - Appels aux services liés à l’élément que l’on souhaite supprimer ;
  - Vérification que tous les éléments liés ont bien été supprimés ;
  - Suppression de l’élément initial.
Ainsi, en cas d’erreurs de la suppression, l’élément initial existe toujours et l’utilisateur peut recommencer à tenter la suppression.


## Auth

Identité et authentification
Projet API, utilise Identity, EF Core, JWT
Gère la connexion à l’application.

- Inscription d’un nouveau propriétaire
- Connexion d’un propriétaire existant
- Déconnexion d’un propriétaire
- Gestion du mot de passe oublié

Le service Auth est le seul service responsable de l’authentification et du stockage des données Identity.
Les autres services utilisent l’authentification JWT afin d’autoriser l’accès à leurs endpoints.
Le service Auth ne gère que les informations utiles à Identity, le profil du propriétaire est géré par le service Profils.


## FrontWeb

Interface utilisateur Web
Projet MVC, utilise JWT
Gère l’interface utilisateur Web de l’application.

Au besoin, le front pourra être divisé en plusieurs projets pour plus de clarté.


## Gateway

Passerelle
Projet minimal, utilise Ocelot, JWT
Sert de passerelle entre les différents projets.


## Notes

Contenu libre et potentiellement pièces jointes
Projet API, utilise MongoDB, JWT
Gère les notes ou remarques faites par le vétérinaire, un autre professionnel ou encore le propriétaire d’un animal lui-même.
Accepte l’ajout de documents (ordonnance, facture, résultat d’analyse, radio, etc.)

- Affichage d’une note
- Affichage de toutes les notes de l’animal
- Ajout d’une note
- Modification d’une note
- Suppression d’une note

Le propriétaire ajoute lui-même ces notes, et non les professionnels qui n’ont pas accès aux données des propriétaires.


## Planning

Calendrier et rappels
Projet API, utilise EF Core, JWT
Gère le planning d’un propriétaire, tel un calendrier réservé à ses animaux.

- Tâche :
  - Affichage d’une tâche d’un animal
  - Affichage de toutes les tâches d’un animal
  - Affichage des tâches de tous les animaux du propriétaire
  - Ajout d’une tâche
  - Modification d’une tâche
  - Suppression d’une tâche

- Rendez-vous :
  - Affichage d’un rendez-vous
  - Affichage de tous les rendez-vous d’un animal
  - Affichage des rendez-vous de tous les animaux du propriétaire
  - Ajout d’un rendez-vous
  - Modification d’un rendez-vous
  - Suppression d’un rendez-vous

Si un praticien est supprimé, les rendez-vous qui lui sont liés voient leur champ de l’Id du propriétaire devenir null afin de ne pas conserver de lien cassé.
Une gestion d’envoi de notifications pourra être ajoutée ultérieurement (rappels, etc.).


## Prévention

Historique vaccinal, vermifuge et antipuce
Projet API, utilise EF Core, JWT
Gère les vaccinations, la prise de vermifuges et d’antipuces de l’animal du propriétaire.

- Vaccin :
  - Ajout d’un vaccin
  - Affichage d’un vaccin
  - Affichage de tous les vaccins
  - Modification d’un vaccin
  - Suppression d’un vaccin

- VermifugeAntipuce:
  - Ajout d’un vermifuge ou d’un antipuce
  - Affichage d’un vermifuge ou d’un antipuce
  - Affichage de tous les vermifuges d’un animal
  - Affichage de tous les antipuces d’un animal
  - Modification d’un vermifuge ou d’un antipuce
  - Suppression d’un vermifuge ou d’un antipuce

Une gestion de création automatique de rappels pourra être ajoutée ultérieurement (rappels de vaccins, prises d’antipuces et de vermifuges).


## Profils

Propriétaires, animaux et praticiens
Projet API, utilise EF Core, JWT
Gère le profil du propriétaire, celui de ses animaux et ceux de leurs praticiens.

- Propriétaire :
  - Affichage de son profil
  - Modification de son profil
  - Suppression de son profil
    - La suppression d’un propriétaire supprime tous les animaux qui lui sont liés, ses notes, ses tâches du planning, ses soins, ses surveillances et ses vaccinations.

- Animal :
  - Ajout d’un animal
  - Affichage de l’animal du propriétaire
  - Affichage de tous les animaux du propriétaire
  - Modification d’un animal du propriétaire
  - Suppression d’un animal du propriétaire
    - La suppression d’un animal supprime ses notes, ses tâches du planning, ses soins, ses surveillances et ses vaccinations qui lui sont liés.

En cas de décès d’un animal, le propriétaire peut archiver son profil au lieu de le supprimer, afin de conserver ses données.
Le propriétaire peut afficher la chronologie médicale de chacun de ses animaux, listant ses vaccins, ses consultations, ses notes, ses traitements, etc.
Le propriétaire peut exporter le carnet de santé de chacun de ses animaux au format PDF.

- Praticien :
  - Ajout d’un praticien
  - Affichage d’un praticien
  - Affichage de tous les praticiens
  - Modification d’un praticien
  - Suppression d’un praticien


## Soins

Ordonnances et traitements
Projet API, utilise EF Core, JWT
Gère les soins à apporter à un animal du propriétaire.

- Ordonnance :
  - Ajout d’une ordonnance
  - Affichage d’une ordonnance
  - Affichage de toutes les ordonnances
  - Modification d’une ordonnance
  - Suppression d’une ordonnance
    - La suppression d’une ordonnance supprime ses traitements et ses prises de traitement qui lui sont liés.

- Traitement :
  - Ajout d’un traitement
  - Affichage d’un traitement
  - Affichage de tous les traitements
  - Modification d’un traitement
  - Suppression d’un traitement
    - La suppression d’un traitement supprime ses prises de traitement qui lui sont liées.

- Prise de traitement
  - Ajout d’une prise de traitement
  - Affichage d’une prise de traitement
  - Affichage de toutes les prises de traitement
  - Modification d’une prise de traitement
  - Suppression d’une prise de traitement


## Surveillances

Données de suivi
Projet API, utilise EF Core, JWT
Gère les surveillances à réaliser sur un animal du propriétaire.

- Surveillance :
  - Ajout d’une surveillance
  - Affichage d’une surveillance
  - Affichage de toutes les surveillances
  - Modification d’une surveillance
  - Suppression d’une surveillance
    - La suppression d’une surveillance supprime ses suivis de surveillance qui lui sont liés.

- Suivi de surveillance :
  - Ajout d’un suivi de surveillance
  - Affichage d’un suivi de surveillance
  - Affichage de tous les suivis de surveillance
  - Modification d’un suivi de surveillance
  - Suppression d’un suivi de surveillance

Inclut une fonctionnalité permettant d’afficher certains suivis sous forme de courbe (comme pour le poids, par exemple).
La gestion du poids inclut le poids de l’animal, le poids minimum recommandé et le poids maximum recommandé.
