# 📘 Guide Complet de Présentation du Projet : Portail SOCADEL

Ce document est conçu pour vous accompagner pas-à-pas dans la présentation orale et la démonstration technique du **Portail SOCADEL** devant votre tuteur. Il combine vulgarisation (pour une personne non technique) et détails précis du code pour répondre à n'importe quelle question.

---

## 📑 Sommaire
1. [Vue d'Ensemble & Objectifs du Projet](#1-vue-densemble--objectifs-du-projet)
2. [Architecture du Projet (Front-End vs Back-End)](#2-architecture-du-projet-front-end-vs-back-end)
3. [Les Fichiers Phares à Projeter dans VS Code](#3-les-fichiers-phares-à-projeter-dans-vs-code)
4. [Où et Comment sont Stockées les Données ?](#4-où-et-comment-sont-stockées-les-données-)
5. [Comment l'Application appelle-t-elle un Rapport Externe ?](#5-comment-lapplication-appelle-t-elle-un-rapport-externe-)
6. [Prise en Charge de SQL Server](#6-prise-en-charge-de-sql-server)
7. [Sécurité, Identifiants & Protection Anti-Piratage](#7-sécurité-identifiants--protection-anti-piratage)
8. [Les APIs et Services Internes](#8-les-apis-et-services-internes)
9. [Scénario Conseillé pour la Démonstration Orale](#9-scénario-conseillé-pour-la-démonstration-orale)

---

## 1. Vue d'Ensemble & Objectifs du Projet

### En 2 phrases :
Le **Portail SOCADEL** est une application web d'entreprise centralisée servant de **guichet unique** pour la direction et les collaborateurs. Il permet d'organiser, de sécuriser et de consulter l'ensemble des tableaux de bord décisionnels (Power BI, SSRS, rapports Excel) selon une arborescence décisionnelle personnalisable.

### Problème résolu :
Avant ce portail, les collaborateurs devaient jongler entre plusieurs liens dispersés ou fichiers Excel reçus par mail. Le Portail SOCADEL centralise tout au même endroit avec un contrôle d'accès strict.

---

## 2. Architecture du Projet (Front-End vs Back-End)

```
┌────────────────────────────────────────────────────────────────────────┐
│                        FRONT-END (Navigateur Web)                      │
│   HTML5 / CSS3 Thème Sombre & Clair / JavaScript / FontAwesome         │
│   - Page de Connexion (/Login)                                         │
│   - Arborescence dynamique & Lecteur de Rapport (/Index)               │
│   - Console d'Administration (/Admin)                                  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Requêtes HTTP (GET / POST)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                     BACK-END (Serveur ASP.NET Core 8.0)                │
│   Moteur C# / Modèle Razor Pages / Injection de Dépendances           │
│   - Security & Authentication (Cookies, SHA-256, Anti-CSRF)            │
│   - Services Internes : INavigationService, IAuthService               │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Entity Framework Core (ORM)
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                      PERSISTANCE & STOCKAGE                            │
│   - Fichier de stockage permanent : portailsocadel_store.json          │
│   - Support natif SQL Server (Production) & SQLite / InMemory          │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Les Fichiers Phares à Projeter dans VS Code

Lorsque votre tuteur vous demande d'ouvrir le code, voici les **6 fichiers stratégiques** à montrer :

| Fichier | Emplacement | Rôle Principal à Expliquer |
| :--- | :--- | :--- |
| **`Program.cs`** | Racine | Point d'entrée de l'application. Configure la sécurité par cookie, le moteur de base de données (SQL Server vs InMemory) et la redirection automatique vers `/Login`. |
| **`appsettings.json`** | Racine | Fichier de configuration JSON. Contient les paramètres du provider de base de données et les chaînes de connexion SQL Server. |
| **`portailsocadel_store.json`** | Racine | Fichier physique de stockage sur disque. Contient tout l'historique permanent des menus, catégories, liens de rapports et utilisateurs. |
| **`Models/MenuItem.cs`** | `Models/` | Classe modèle C# représentant la structure d'un élément (Catégorie, Sous-catégorie ou Rapport) avec son type, son moteur et son URL externe (`ReportUrl`). |
| **`Services/DbNavigationService.cs`** | `Services/` | Service central (API interne) qui exécute la logique métier : ajout, modification, suppression d'éléments et déclenchement de la sauvegarde sur disque. |
| **`Pages/Index.cshtml`** | `Pages/` | La vue HTML/Razor qui génère le composant `<iframe src="...">` pour afficher le rapport Power BI / SSRS sélectionné. |

---

## 4. Où et Comment sont Stockées les Données ?

Si votre tuteur demande : **« Où stockes-tu les rapports, menus et utilisateurs ? »**, voici la réponse exacte :

### 1. Le Fichier de Stockage Physique (`portailsocadel_store.json`)
* **Ce qu'il contient :** L'intégralité du magasin de données en format JSON lisible.
* **Sa fonction :** Dès qu'un administrateur ajoute, modifie ou supprime un élément dans la console d'administration, `DbSeeder.SaveData()` écrit immédiatement l'état à jour dans ce fichier.
* **Persistance Totale :** Au démarrage du serveur (`dotnet run`), l'application lit ce fichier et réhydrate la mémoire. Toutes les modifications restent conservées **indéfiniment**, même après des jours ou des redémarrages serveur.

### 2. Le Mapping de Base de Données (`Data/AppDbContext.cs`)
* **Ce qu'il contient :**
  ```csharp
  public DbSet<MenuItem> MenuItems { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;
  ```
* **Sa fonction :** C'est la classe **Entity Framework Core** qui sert de passerelle universelle entre nos modèles C# et la base de données.

---

## 5. Comment l'Application appelle-t-elle un Rapport Externe ?

Expliquez cette chaîne en 3 étapes claires :

### Étape A : Le Modèle (`Models/MenuItem.cs`)
Chaque rapport possède un attribut `ReportUrl` (l'adresse web du rapport Power BI ou SSRS) et un type `Engine` (ex: `PowerBI`, `SSRS`, `Excel`).

### Étape B : Le Traitement Back-End (`Pages/Index.cshtml.cs`)
Quand un utilisateur clique sur un rapport dans l'arborescence (URL: `/?report=rep-enc-synth`) :
```csharp
public void OnGet([FromQuery] string? report)
{
    // Le Back-End interroge le service pour récupérer le rapport par son ID
    CurrentReport = _navService.GetItemById(report);
}
```

### Étape C : L'Affichage Front-End (`Pages/Index.cshtml`, Lignes 130-158)
Le code Razor vérifie la présence d'une URL et génère dynamiquement une balise `<iframe src="...">` :
```html
@if (!string.IsNullOrEmpty(Model.CurrentReport?.ReportUrl))
{
    <div class="pbi-iframe-wrapper">
        <iframe src="@Model.CurrentReport.ReportUrl" 
                title="@Model.CurrentReport.Title"
                width="100%" 
                height="100%" 
                frameborder="0" 
                allowFullScreen="true">
        </iframe>
    </div>
}
```
* **Phrase d'explication pour le tuteur :**
  > *"Le Portail SOCADEL ne stocke pas les données brutes du rapport Power BI dans sa base, mais ses métadonnées. Quand l'utilisateur clique, notre moteur C# injecte l'URL sécurisée dans une fenêtre virtuelle HTML5 (`<iframe>`). Le rapport s'exécute ainsi en direct dans notre portail sans que l'utilisateur n'ait à quitter le site."*

---

## 6. Prise en Charge de SQL Server

Si le tuteur demande : **« Est-ce compatible avec SQL Server ? »**, la réponse est **OUI, à 100%**.

### Comment c'est codé dans `Program.cs` :
```csharp
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "InMemory";
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseInMemoryDatabase("PortailSocadelDb");
    }
});
```

### Comment activer SQL Server en Production ?
Dans `appsettings.json`, il suffit de changer une seule ligne :
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=PortailSocadelDb;User Id=sa;Password=Socadel2024!;TrustServerCertificate=True;"
  }
}
```
* **Phrase d'explication pour le tuteur :**
  > *"Grâce à la puissance d'Entity Framework Core (ORM), basculer entre le mode de développement local et le serveur d'entreprise SQL Server se fait en modifiant un simple paramètre dans `appsettings.json`, sans réécrire une seule ligne de code C#."*

---

## 7. Sécurité, Identifiants & Protection Anti-Piratage

1. **Hachage des Mots de Passe (SHA-256) :**
   Les mots de passe ne sont jamais stockés en texte clair. Dans `Services/AuthService.cs`, ils sont hachés avec l'algorithme cryptographique **SHA-256** (`Convert.ToBase64String(sha256.ComputeHash(...))`).
2. **Authentification par Cookie & Redirection Stricte :**
   Toute personne non authentifiée qui tente d'accéder à la racine `/` ou à `/Admin` est automatiquement redirigée vers la page de connexion `/Login` via `options.Conventions.AuthorizeFolder("/")`.
3. **Protection Anti-CSRF (`__RequestVerificationToken`) :**
   Tous les formulaires dynamiques d'administration incluent un jeton anti-contrefaçon généré par ASP.NET Core (`Antiforgery`), empêchant tout piratage ou exécution de requêtes malveillantes externes (résolvant l'erreur HTTP 400).

---

## 8. Les APIs et Services Internes

### Distinction clé à expliquer au tuteur :
* **Pas d'API REST publique exposée sur l'extérieur :** Cela réduit la surface d'attaque et protège les données décisionnelles de SOCADEL.
* **Architecture orientée Services Internes (Inversion de Contrôle / DI) :**
  - `INavigationService` / `DbNavigationService` : Service gérant la hiérarchie et les opérations CRUD sur le menu.
  - `IAuthService` / `AuthService` : Service gérant la vérification d'identité et la création de compte.
* **Handlers Razor (Endpoints Web) :**
  Les interactions de formulaires (`OnPostAdd`, `OnPostDelete`, `OnPostReset`) fonctionnent comme des endpoints d'API internes sécurisés.

---

## 9. Scénario Conseillé pour la Démonstration Orale

1. **Étape 1 : Démonstration visuelle (2-3 min)**
   - Ouvrir la page de connexion `/Login`.
   - Se connecter avec le compte Admin (`admin@socadel.cm`).
   - Parcourir l'arborescence des rapports (Commercial, Finance, etc.) et montrer le chargement interactif d'un rapport Power BI.
   - Aller sur la console Admin, créer une nouvelle catégorie (ex: *Direction Technique*), puis supprimer un élément de test pour prouver l'absence d'erreur HTTP 400.

2. **Étape 2 : Démonstration de la Persistance (1 min)**
   - Arrêter le serveur dans le terminal (`Ctrl+C`).
   - Le relancer (`dotnet run`).
   - Rafraîchir la page et montrer que l'élément créé est **toujours présent**.

3. **Étape 3 : Démonstration du Code dans VS Code (3-4 min)**
   - Ouvrir `Program.cs` pour montrer la sécurité et la flexibilité SQL Server.
   - Ouvrir `portailsocadel_store.json` pour montrer où les données sont conservées.
   - Ouvrir `Pages/Index.cshtml` pour montrer l'intégration `<iframe src="...">` du rapport externe.
