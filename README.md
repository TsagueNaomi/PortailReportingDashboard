# Portail de Reporting et Dashboard - SOCADEL

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Razor%20Pages-512BD4?style=flat&logo=.net&logoColor=white)](https://learn.microsoft.com/aspnet/core/razor-pages/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=flat&logo=.net&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server-CC292B?style=flat&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server/)
[![Language](https://img.shields.io/badge/Language-C%23%2012-239120?style=flat&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)

Le **Portail SOCADEL** est une application web qui permet d'accéder de façon sécurisée aux tableaux de bord et aux rapports de la société **SOCADEL** (*Société Camerounaise d'Electricité*).

- **Frontend :** HTML5, CSS3 et JavaScript pour l'affichage des pages, les menus et la navigation.
- **Backend :** C# 12 et ASP.NET Core 8 pour la gestion des utilisateurs, des rôles et de la sécurité.
- **Base de données :** SQL Server pour le stockage de toutes les données de l'application.

---

## Fonctionnalités Principales

- **Connexion et gestion des accès**
  - Connexion sécurisée par identifiant et mot de passe.
  - Deux niveaux d'accès : **Administrateur** (gestion complète) et **Utilisateur** (consultation).
- **Navigation structurée sur 3 niveaux**
  - Organisation des contenus en menus, sous-menus et rapports.
  - Possibilité d'ajouter, modifier ou supprimer des éléments directement depuis l'interface d'administration.
- **Interface moderne et responsive**
  - Design personnalisé avec barre de recherche et fil d'Ariane pour faciliter la navigation.

---

## Configuration de la base de données

Le fichier `appsettings.json` permet de configurer la connexion à la base de données :

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=PortailSocadelDb;User Id=sa;Password=Socadel2024!;TrustServerCertificate=True;"
  }
}
```

Pour utiliser SQL Server en local :
1. Indiquez `"DatabaseProvider": "SqlServer"` dans le fichier de configuration.
2. Assurez-vous que SQL Server est bien démarré (`sudo systemctl start mssql-server`).

---

## Demarrage

```bash
# 1. Compiler le projet
dotnet build

# 2. Lancer l'application
dotnet run
```

Accedez ensuite a l'application dans votre navigateur : **`http://localhost:5138`**

---

© **SOCADEL - Societe Camerounaise d'Electricite**
