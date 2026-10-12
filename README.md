# Sensori

Projet Unity 6000.6.2f1. L'identifiant Android reste `com.florian.sensori`.

## Publication de l'APK Android

À chaque push sur `main`, le workflow [`.github/workflows/android-release.yml`](.github/workflows/android-release.yml) construit un APK signé avec [GameCI](https://game.ci/docs/github/builder) (`game-ci/unity-builder`) et le publie dans les [Releases GitHub](https://github.com/floflo713-stack/Sensori/releases).

Le tag et la version affichée sont `v1.0.<numéro d'exécution>` (par exemple `v1.0.3`). Le `versionCode` Android prend la même valeur, pour qu'Android accepte la mise à jour par-dessus l'installation précédente.

Le dossier `Library` est mis en cache entre les builds. Aucun mot de passe, fichier `.ulf` ou keystore ne doit être commité.

### Activer une licence Unity Personal gratuite

GameCI ne peut pas créer la licence à ta place. Il faut l'activer une fois sur ta machine, puis la déposer dans les secrets du dépôt.

1. Installe [Unity Hub](https://unity.com/download) et connecte-toi avec le compte Unity qui servira à la CI.
2. Ouvre **Unity Hub → Preferences → Licenses** (Préférences → Licences), clique sur **Add**, puis choisis **Get a free personal license**.
3. Clique bien sur **Add** et termine l'activation, même si une licence est déjà listée. Sinon le fichier `.ulf` n'est pas toujours créé.
4. Récupère ce fichier :
   - Windows : `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS : `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux : `~/.local/share/unity3d/Unity/Unity_lic.ulf`
5. Copie **tout** son contenu dans le secret `UNITY_LICENSE`.
6. Renseigne aussi `UNITY_EMAIL` et `UNITY_PASSWORD` avec le même compte. GameCI s'en sert pour réactiver la licence Personal pendant le build.

Le mot de passe doit de préférence être alphanumérique, sans caractère spécial : GameCI a déjà vu des activations échouer à cause de caractères spéciaux.

`UNITY_LICENSE` est obligatoire pour une licence Personal. L'e-mail et le mot de passe seuls ne suffisent pas : l'action s'arrête s'il manque le fichier `.ulf`.

### Générer le keystore et l'encoder en base64

Il faut un JDK, pour avoir la commande `keytool`. Choisis un mot de passe de keystore et un mot de passe de clé, et retiens l'alias (`sensori` dans l'exemple).

```bash
keytool -genkeypair -v \
  -keystore sensori-release.keystore \
  -alias sensori \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000
```

Encode ensuite le fichier **sans retours à la ligne** :

```bash
# Linux
base64 -w 0 sensori-release.keystore

# macOS
base64 -i sensori-release.keystore | tr -d '\n'
```

Colle le résultat dans `ANDROID_KEYSTORE_BASE64`. Ne commite pas `sensori-release.keystore` : il est ignoré par git, comme les fichiers `.jks` et `.ulf`.

### Secrets GitHub à créer

Dans le dépôt : **Settings → Secrets and variables → Actions → New repository secret**.

Les noms doivent être exactement ceux-ci :

| Secret | Contenu |
| --- | --- |
| `UNITY_LICENSE` | Contenu complet du fichier `.ulf` |
| `UNITY_EMAIL` | E-mail du compte Unity Personal |
| `UNITY_PASSWORD` | Mot de passe de ce compte |
| `ANDROID_KEYSTORE_BASE64` | Keystore encodé en base64, sur une seule ligne |
| `ANDROID_KEYSTORE_PASS` | Mot de passe du keystore (`keytool`) |
| `ANDROID_KEYALIAS_NAME` | Alias de la clé, par exemple `sensori` |
| `ANDROID_KEYALIAS_PASS` | Mot de passe de la clé |

Ajoute ces secrets avant de merger sur `main`. Le premier push sans secrets fera échouer le build.

### Installer l'APK sur un téléphone Android

1. Ouvre l'onglet **Releases** du dépôt GitHub et télécharge `Sensori-v1.0.N.apk`.
2. Autorise l'installation hors du Play Store :
   - Android 8 et suivants : au moment du téléchargement, autorise le navigateur ou l'application Fichiers via **Paramètres → Applications → Accès spécial → Installer des applications inconnues**.
   - Versions plus anciennes : **Paramètres → Sécurité → Sources inconnues**.
3. Ouvre le fichier APK et confirme l'installation.

Les releases suivantes s'installent par-dessus l'application déjà présente, parce que l'identifiant reste `com.florian.sensori`, la clé de signature est la même, et le `versionCode` augmente. Si Android refuse la mise à jour à cause d'un conflit de signature (ancienne version signée avec une autre clé), désinstalle Sensori puis réinstalle l'APK. Les données locales de l'application sont alors perdues.
