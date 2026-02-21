# 🏰 e-Daemon API WebServer 🎲

*Um grimório arcano de back-end, forjado nas chamas do .NET 8 e abastecido por uma quantidade profana de café.*

Bem-vindo, ó nobre aventureiro do código! Adentre o domínio do **e-Daemon**, um servidor web que não apenas serve endpoints, mas também conta histórias de reinos distantes e proezas heróicas. Se você procura uma API RESTful com a força de um dragão e a sabedoria de um arquimago, seu lugar é aqui. Pegue sua caneca de café mais robusta e prepare-se para a jornada.

![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![C#](https://img.shields.io/badge/C%23-12-blue)
![Status](https://img.shields.io/badge/status-em_desenvolvimento-red)
![License](https://img.shields.io/badge/license-GPL--2.0-blue)

## 📜 A Lenda do e-Daemon

Reza a lenda que, em uma era onde os servidores eram lentos e as requisições se perdiam na escuridão, um programador-feiticeiro decidiu dar um basta. Após sete dias e sete noites de codificação ininterrupta, alimentado apenas por café expresso e o desejo de ordem, nasceu o Projeto e-Daemon, baseado no Sistema Daemon, RPG criado no reino de Pindorama (conhecido como Brasil), criado pelo arquimago Erasmus, também conhecido como Marcelo Del Debbio, em 1995.

Você pode encontrar mais informações sobre este incrível sistema analógico de RPG em sua [biblioteca](https://wiki.daemon.com.br/index.php/Sistema_Daemon), mantida pela ordem da Editora Daemon.

Este projeto é a encarnação desse espírito: uma API robusta, escalável e (ousamos dizer) mágica, projetada para ser a espinha dorsal de aplicações lendárias, especialmente aquelas que envolvem dados de RPG.

## 🧙 O Grimório do Mago: Arquitetura

Nossa magia é organizada. Seguimos os antigos ensinamentos da arquitetura em camadas para garantir que cada feitiço (ou lógica de negócio) seja puro e testável.

- **`Controllers` (Os Arautos do Reino):** Nossos endpoints REST. Eles recebem os chamados do mundo exterior e os direcionam para os magos adequados.
- **`Services` (O Conselho dos Arquimagos):** Onde a verdadeira mágica acontece. A lógica de negócio, as regras e a orquestração de feitiços residem aqui.
- **`Repositories` (Os Guardiões do Conhecimento):** Abstrações para o acesso aos dados. Eles conversam com os tomos antigos (bancos de dados) para que nossos magos não precisem sujar as mãos.
- **`Domain` (Os Manuscritos Sagrados):** Nossas entidades e DTOs. A essência de nossas habilidades, itens e monstros.

## ✨ Invocações e Feitiços: Endpoints da API

Você pode se comunicar com o e-Daemon usando as seguintes invocações HTTP:

### Perícias Básicas (`Basic Skills`)

- **`GET /api/skill/basic-skills`**
  - **Descrição:** Conjura uma lista de todas as habilidades básicas conhecidas no reino.
  - **Filtros:** Permite filtrar a busca como um verdadeiro Sábio.

- **`GET /api/skill/basic-skills/{id}`**
  - **Descrição:** Busca por uma habilidade específica em nossos anais, usando seu `id` único.

## 🛠️ Como conjurar o e-Daemon: Executando o Projeto

Para trazer o e-Daemon ao seu plano de existência, siga seguintes passos ritualísticos:

1.  **Obtenha o Pergaminho:** Clone este repositório.
    ```bash
    git clone https://github.com/ratto/EDaemonWebServer.git
    ```
2.  **Entre no Santuário:** Navegue até o diretório do projeto.
3.  **Execute o Ritual:** Abra a solução (`EDaemon.sln`) no Visual Studio 2022 (ou superior) e pressione `F5`. Opcionalmente, use o comando `dotnet run` no diretório `EDaemonWebServer`.

Se o Ritual funcionar, o e-Daemon despertará, e a documentação dos seus feitiços (Swagger) estará disponível em seu `localhost`.

##  📖 A Forja dos Anões: Contribuindo

Sentiu o chamado para forjar novas armas ou aprimorar os feitiços existentes? Contribuições são a alma desta fortaleza! Sinta-se à vontade para abrir `Pull Requests` com melhorias, correções de bugs ou novas funcionalidades. Toda ajuda é bem-vinda para tornar este grimório ainda mais poderoso.

## 📜 O Pacto Antigo: Licença

Este projeto é regido pelo antigo e honrado pacto **GPL v2**. Consulte o arquivo `LICENSE.txt` para conhecer seus direitos e deveres.

## 📧 Contato

- **Pedro "Ratto" Paixão**
  - **GitHub:** [@ratto](https://github.com/ratto)
  - **LinkedIn:** [pedro-paixao](https://www.linkedin.com/in/pedro-paixao/)

## 💖 Apoie este Projeto

Se o e-Daemon tem sido útil em suas aventuras, considere apoiar o projeto. Qualquer ajuda acelera o desenvolvimento de novas magias e a manutenção do nosso castelo de código.

- **Dê uma estrela no GitHub:** É o jeito mais simples e rápido de mostrar seu apoio.
- **Abra `Issues` e `Pull Requests`:** Ajude a encontrar bugs e aprimorar o código.
- **Compartilhe com seus companheiros:** Espalhe a palavra sobre o e-Daemon!
- **Doe via PayPal:** Para manter o fluxo de café e a sanidade do mago.
  - [![PayPal](https://img.shields.io/badge/PayPal-doar-blue.svg?logo=paypal)](https://www.paypal.com/donate/?hosted_button_id=BFE8B3R8Q8L4N)

---

> *Que seus builds compilem na primeira tentativa e seu café nunca esfrie.*