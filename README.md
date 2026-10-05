# GrupoJap Rentals

Painel de administração para gerir veículos, clientes e contratos de aluguer, criado como desafio técnico para o GrupoJap. A especificação funcional está em [docs/requisitos-projeto.pdf](docs/requisitos-projeto.pdf).

## Tecnologias

- ASP.NET Core MVC (C#, .NET 10) com views Razor, HTML, CSS e JavaScript.
- Entity Framework Core 10 com SQL Server, abordagem Code-First (migrations).
- ASP.NET Core Identity (perfis e login).
- xUnit para os testes automatizados.

## Estrutura

| Projeto | Função |
|---|---|
| `src/GrupoJap.Rentals.Admin` | Aplicação web: controllers e views do painel |
| `src/GrupoJap.Rentals.Core` | Modelos, `ApplicationDbContext`, migrations, serviços, traduções, login/perfil e assets comuns |
| `tests/GrupoJap.Rentals.Tests` | Testes xUnit |

## Pontos cruciais

- **Gestão completa** de veículos, clientes e contratos (listar, criar, editar, eliminar), com validação no servidor e no cliente.
- **Contratos sem conflitos:** as datas são inclusivas, por isso dois contratos do mesmo veículo que partilhem um dia sobrepõem-se. O início não pode ser anterior a hoje e o fim tem de ser posterior ao início.
- **Estado calculado, não guardado:** um veículo está *Alugado* se tiver um contrato com `início ≤ hoje ≤ fim`; caso contrário, *Disponível*.
- **Unicidade:** matrícula e email únicos, também garantidos por índice na base de dados.
- **Integridade:** não se elimina um veículo ou cliente que tenha contratos associados.
- **Telefone português:** 9 dígitos, a começar por 9.
- **Acesso restrito a administradores**, com bloqueio de conta após 5 tentativas falhadas. Não há registo público.
- **Três idiomas (PT, EN, ES):** botão junto ao modo noturno. Os textos ficam na base de dados e editam-se em *Traduções*; os valores iniciais estão em `Core/Localization/TranslationCatalog.cs`. Para um texto novo, criar a entrada no catálogo e usar `@T["chave"]` na view.
- **Modo claro/noturno** e perfil de utilizador com foto.

## Como correr

Pré-requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download) e uma instância SQL Server (por omissão, `localhost\SQLEXPRESS` com autenticação do Windows).

```powershell
dotnet restore
dotnet tool restore
dotnet ef database update --project src/GrupoJap.Rentals.Core --startup-project src/GrupoJap.Rentals.Admin
dotnet run --project src/GrupoJap.Rentals.Admin
```

Abre http://localhost:5169. No arranque são criados o administrador e os dados de demonstração; as credenciais estão na secção `SeedAdmin` de `src/GrupoJap.Rentals.Admin/appsettings.Development.json`.

- **Outra base de dados:** altera a `DefaultConnection` no mesmo ficheiro. Nunca apliques migrations numa base com dados sem rever o destino.
- **Credenciais:** não coloques credenciais reais em ficheiros versionados; em ambientes partilhados usa User Secrets ou variáveis de ambiente.
- **Nova migration:** `dotnet ef migrations add Nome --project src/GrupoJap.Rentals.Core --startup-project src/GrupoJap.Rentals.Admin`

## Testes

```powershell
dotnet test
```

Não precisam de SQL Server (usam uma base em memória). Cobrem as regras de datas e disponibilidade, as validações dos modelos, os controllers de veículos, clientes, alugueres e painel, e o sistema de traduções. Com o Admin a correr, a DLL fica bloqueada: pára a aplicação ou usa `dotnet test --artifacts-path .\.artifacts`.
