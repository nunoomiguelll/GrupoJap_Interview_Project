# GrupoJap Rentals

Aplicacao web para gestao de veiculos, clientes e contratos de aluguer, criada como desafio tecnico para o GrupoJap.

## Base tecnica

- ASP.NET Core MVC em C# com .NET 10.
- Razor, HTML, CSS e JavaScript no frontend.
- Entity Framework Core 10 com SQL Server e abordagem Code-First.
- Testes automatizados previstos com xUnit.

O projeto MVC, as entidades iniciais (`Vehicle`, `Customer` e `RentalContract`), o `ApplicationDbContext` e as migrations `InitialCreate` e `LimitPhoneLength` ja estao criados. A base `GrupoJapRentals` foi criada na instancia local SQL Server Express e as migrations foram aplicadas. Ainda nao existem ecras de gestao. Consulte [a especificacao funcional em PDF](docs/requisitos-projeto.pdf).

## Estrutura da solucao

A solucao (`GrupoJap.slnx`) tem tres projetos que partilham a mesma base de dados mas correm como aplicacoes independentes:

| Projeto | Funcao | URL (dev) |
|---|---|---|
| `src/GrupoJap.Rentals.Web` | Site publico (frota, reservas, conta do cliente) | http://localhost:5168 |
| `src/GrupoJap.Rentals.Admin` | Painel de administracao (veiculos, clientes, contratos) | http://localhost:5169 |
| `src/GrupoJap.Rentals.Core` | Biblioteca partilhada: modelos, `ApplicationDbContext`, migrations, servicos, login/perfil e assets comuns (CSS/JS/bootstrap) | n/a |

- Cada aplicacao tem sessao propria (cookies distintos). O Admin so aceita contas com o perfil `Admin` e nao permite registo; o Web permite registo de clientes.
- As fotos de perfil ficam em `uploads/` na raiz da solucao (`Uploads:Path`), servidas por ambas em `/uploads`.
- Os links entre as duas apps vem de `Urls:Admin` (Web) e `Urls:Site` (Admin) nos `appsettings.json`.

## Configuracao local

A connection string de desenvolvimento esta em `appsettings.Development.json` de cada app e usa SQL Server Express (`localhost\SQLEXPRESS`) com autenticacao do Windows. O utilizador administrador e os dados de demonstracao (`SeedAdmin`, `SeedDemoData`) sao criados pelo Admin ao arrancar.

```powershell
dotnet restore
dotnet tool restore
dotnet ef database update --project src/GrupoJap.Rentals.Core --startup-project src/GrupoJap.Rentals.Admin

# correr so uma das aplicacoes (ou as duas, em terminais separados)
dotnet run --project src/GrupoJap.Rentals.Web
dotnet run --project src/GrupoJap.Rentals.Admin

dotnet test tests/GrupoJap.Rentals.Tests
```

Nova migration: `dotnet ef migrations add Nome --project src/GrupoJap.Rentals.Core --startup-project src/GrupoJap.Rentals.Admin`.

Para outra instancia SQL Server, substitui `Server=localhost\SQLEXPRESS` nos dois `appsettings.Development.json`. Nunca apliques migrations numa base existente com dados sem rever o destino.

Nao colocar credenciais reais em ficheiros versionados. Para ambientes partilhados, usar User Secrets ou variaveis de ambiente.

## Planeamento de implementacao

### Fase 1 - Dominio e persistencia

- Dominio, `ApplicationDbContext`, relacoes, indices unicos e injecao de dependencia ja configurados.
- Base `GrupoJapRentals` criada na instancia local SQL Server Express e migrations aplicadas.
- Rever o esquema criado e documentar como recriar a base de dados noutro ambiente.

### Fase 2 - Veiculos e clientes

- Implementar CRUD MVC, formularios e listagens para veiculos e clientes.
- Validar campos obrigatorios, ano de fabrico nao futuro, matricula e email unicos, email valido e telefone numerico em formato aceite.
- Tratar duplicados tambem na persistencia, para que a regra nao dependa apenas da validacao do formulario.

### Fase 3 - Contratos e disponibilidade

- Criar contratos associados a um cliente e a um veiculo, com inicio, fim e quilometragem inicial.
- Validar inicio nao anterior ao dia atual, fim posterior ao inicio e quilometragem nao negativa.
- Impedir alugueres sobrepostos para o mesmo veiculo. As datas sao inclusivas: ha conflito quando `inicioExistente <= fimNovo` e `fimExistente >= inicioNovo`.
- Calcular o estado como `Alugado` quando existir contrato com `inicio <= hoje <= fim`; caso contrario, `Disponivel`. Nao guardar esse estado como dado independente.

### Fase 4 - Experiencia de utilizacao

- Integrar navegacao e identidade visual do desafio no layout MVC.
- Completar listagens de veiculos, clientes e contratos com estado, mensagens de validacao e confirmacao de operacoes.
- Rever acessibilidade, apresentacao responsiva e validacoes no servidor; usar validacao JavaScript como complemento, nunca como unica protecao.

### Fase 5 - Qualidade e entrega

- Criar testes unitarios para regras de datas, sobreposicao de contratos, disponibilidade e validacoes relevantes.
- Executar build e testes, percorrer os fluxos principais e corrigir os problemas encontrados.
- Documentar configuracao SQL Server, migrations, decisoes de arquitetura e roteiro de demonstracao.
- Manter commits pequenos e descritivos ao longo do desenvolvimento.

## Sequencia sugerida para os 5 dias

1. Modelo de dominio, EF Core, SQL Server e migration inicial.
2. CRUD de veiculos e clientes, incluindo validacoes e unicidade.
3. CRUD de contratos e regras de disponibilidade/nao sobreposicao.
4. Layout, navegacao, listagens e revisao dos fluxos da aplicacao.
5. Testes, documentacao, correcao de defeitos e preparacao da apresentacao.

## Decisoes a confirmar durante a implementacao

- Formato de telefone aceite e se a aplicacao assume exclusivamente numeros portugueses.
- Se a disponibilidade deve considerar apenas alugueres ativos hoje ou tambem reservas futuras. O plano considera o estado atual para a listagem e bloqueia sobreposicoes futuras ao criar contratos.
- A infraestrutura SQL Server disponivel para desenvolvimento e apresentacao.
- Integracoes reutilizaveis que serao fornecidas posteriormente.
## Estado de implementacao (atualizado)

- **Feito:** CRUD completo de Veiculos, Clientes e Contratos (listar, criar, editar, eliminar com confirmacao); validacoes de servidor e cliente; unicidade de matricula e email (tambem tratada na persistencia); bloqueio de contratos sobrepostos por veiculo; estado Disponivel/Alugado calculado a partir dos contratos; eliminacao bloqueada quando existem contratos associados; navegacao ativa no layout.
- **Regras puras:** `src/GrupoJap.Rentals.Core/Services/RentalRules.cs` (sobreposicao, contrato ativo, estado) com testes xUnit em `tests/GrupoJap.Rentals.Tests` (`dotnet test tests/GrupoJap.Rentals.Tests`).
- **Por fazer:** pesquisa/filtros nas listagens, paginacao, seed de dados de demonstracao, testes de integracao dos controladores, protecao contra concorrencia na criacao de contratos (hoje a verificacao de sobreposicao e feita na aplicacao, nao por restricao na BD), integracoes externas (Postmark, Stripe) adiadas.
- **Nota Git:** `bin/` e `obj/` ja estavam versionados; o `.gitignore` novo nao os remove do indice. Para parar de os versionar: `git rm -r --cached bin obj`.

## Autenticacao e perfis

- ASP.NET Core Identity com perfis `Admin` e `User` (`Models/AppRoles.cs`). Painel (Painel, Veiculos, Clientes, Alugueres) so acessivel a `Admin`; utilizadores normais veem apenas `/account`.
- O registo publico (`/account/register`) cria sempre utilizadores `User`. O perfil `Admin` so e atribuido internamente.
- O administrador inicial e criado no arranque por `Data/IdentitySeeder.cs` a partir da seccao `SeedAdmin` de `appsettings.Development.json` (`admin@admin.com`). Em producao definir `SeedAdmin__Email` e `SeedAdmin__Password` por variaveis de ambiente/User Secrets e **alterar a palavra-passe**; nao versionar credenciais reais.
- Bloqueio de conta apos 5 tentativas falhadas (5 minutos). Migration: `AddIdentity`.

## Perfil do utilizador

- Menu lateral: **O meu perfil** (`/profile`). O nome, a foto e o cargo apresentados no menu e no topo vêm do próprio utilizador autenticado.
- Campos do utilizador (`ApplicationUser`): nome completo, telefone, cargo, marca favorita (sugestões a partir da frota), bio e foto. Migration `AddUserProfileFields`.
- Fotos: JPG/PNG/WebP até 2 MB, validadas pelos primeiros bytes do ficheiro (não pela extensão), gravadas em `wwwroot/uploads/avatars` (ignorado pelo Git). A foto anterior é apagada ao substituir ou remover.
- Alterar palavra-passe em `/profile/password`.

## Site publico (inspirado em benecar.pt)

- Rotas: `/` (inicio), `/viaturas` (frota com filtros e paginacao), `/viaturas/{id}` (detalhe + reserva), `/conta/reservas` (area de cliente). O painel passou para `/admin`.
- Disponibilidade calculada a partir dos contratos (datas inclusivas) em `Services/RentalAvailabilityService.cs`, partilhado com o backoffice.
- Reserva online (`Services/BookingService.cs`): aplica as regras do PDF (inicio >= hoje, fim > inicio, sem sobreposicao), cria a ficha de cliente na primeira reserva (telefone e carta obrigatorios) e usa a quilometragem atual do veiculo como quilometragem inicial. Verificacao e insercao numa transacao `Serializable`.
- Cancelamento pelo cliente apenas antes do inicio do aluguer.
- Ficha de cliente associada a conta pelo email.
- `SeedDemoData` (so em desenvolvimento) cria frota, clientes e contratos de demonstracao quando a frota esta vazia.
- Estilos em `wwwroot/css/public.css` (tons do backoffice + acento neon).

## Video do hero

- `wwwroot/media/hero-sportscar.mp4`: "A red sports car traveling along a curvy asphalt road" ([Mixkit](https://mixkit.co/free-stock-video/a-red-sports-car-traveling-along-a-curvy-asphalt-road-52427/), licenca Mixkit: uso livre sem atribuicao obrigatoria). Para trocar, substituir o ficheiro (ou alterar a `source` em `Views/Site/Index.cshtml`).
- Reproduz em loop, sem som, e pausa quando sai do ecra. Com "reduzir movimento" ativo no sistema, nao e carregado e fica o fundo em gradiente.

## Idiomas e traducoes

- O site publico esta em **portugues, ingles e espanhol**. O idioma escolhe-se no seletor do cabecalho (guardado num cookie); na primeira visita segue o idioma do browser. A administracao fica sempre em portugues.
- O modo noturno existe apenas na administracao (`Ui:DarkMode` no `appsettings.json` do Admin).
- Os textos vivem nas tabelas `Translations` (chave, categoria, descricao) e `TranslationValues` (um texto por idioma), seguindo o modelo do projeto base.
- **Admin > Traducoes** (`/translations`) permite pesquisar, filtrar por categoria ou por textos em falta, editar, criar e eliminar. O site reflete as alteracoes em ~10 s (`Translations:CacheSeconds`).
- Os valores iniciais estao em `src/GrupoJap.Rentals.Core/Localization/TranslationCatalog.cs`. Ao arrancar, as chaves em falta sao inseridas na base de dados sem alterar o que ja foi editado. Eliminar uma traducao do catalogo equivale a repor o texto original.
- Ordem de recurso de cada texto: BD (idioma atual) -> catalogo (idioma atual) -> BD (portugues) -> catalogo (portugues) -> chave.
- Nas vistas: `@T["chave"]` ou `@T["chave", argumento]`; nas DataAnnotations usa-se a chave em `ErrorMessage`/`Display(Name)`.
