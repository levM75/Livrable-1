CREATE DATABASE livInParis;
USE livInParis;
CREATE TABLE Utilisateur (
    id_utilisateur INT AUTO_INCREMENT PRIMARY KEY,
    mdp_utilisateur VARCHAR(255) NOT NULL,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    adresse TEXT,
    telephone VARCHAR(20),
    adresse_mail VARCHAR(100) UNIQUE NOT NULL
);

CREATE TABLE Menu (
    id_menu INT AUTO_INCREMENT PRIMARY KEY,
    entree VARCHAR(255),
    plat_principal VARCHAR(255),
    repas VARCHAR(255)
);

CREATE TABLE RecetteConnu (
    id_recette_connu INT AUTO_INCREMENT PRIMARY KEY,
    nom_recette VARCHAR(255),
    temps INT
);

CREATE TABLE RecetteOriginale (
    id_recette_originale INT AUTO_INCREMENT PRIMARY KEY,
    temps INT,
    composition TEXT
);

CREATE TABLE Regime (
    id_regime INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100),
    type_regime VARCHAR(100),
    ingredients_non_autorises TEXT
);

CREATE TABLE Ingredient (
    id_ingredient INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100),
    quantite INT
);
CREATE TABLE Particulier (
    id_particulier INT PRIMARY KEY,
    FOREIGN KEY (id_particulier) REFERENCES Utilisateur(id_utilisateur)
);

CREATE TABLE EntrepriseLocale (
    id_entreprise INT PRIMARY KEY,
    nom_referent VARCHAR(100),
    mdp_entreprise VARCHAR(255) NOT NULL,
    nom_entreprise VARCHAR(100),
    numero_telephone_referent VARCHAR(20),
    FOREIGN KEY (id_entreprise) REFERENCES Utilisateur(id_utilisateur)
);

CREATE TABLE Cuisinier (
    id_cuisinier INT PRIMARY KEY,
    FOREIGN KEY (id_cuisinier) REFERENCES Utilisateur(id_utilisateur)
);
CREATE TABLE Commande (
    id_commande INT AUTO_INCREMENT PRIMARY KEY,
    date_commande DATE NOT NULL,
    montant_commande DECIMAL(10,2) NOT NULL,
    id_utilisateur INT NOT NULL,
    FOREIGN KEY (id_utilisateur) REFERENCES Utilisateur(id_utilisateur)
);

CREATE TABLE plat (
    id_plat INT AUTO_INCREMENT PRIMARY KEY,
    Quantite INT NOT NULL,
    date_fabrication DATE NOT NULL,
    date_peremption DATE NOT NULL,
    prix_par_personne DECIMAL(10,2) NOT NULL,
    nationalite_plat VARCHAR(100),
    regime_alimentaire VARCHAR(255),
    photo TEXT
);

CREATE TABLE Livraison (
    id_livraison INT AUTO_INCREMENT PRIMARY KEY,
    date_livraison DATE NOT NULL,
    contenu TEXT,
    trajet TEXT
);
SHOW TABLES;
SELECT * FROM Utilisateur;
SELECT * FROM plat;



