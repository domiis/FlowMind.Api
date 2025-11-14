# FlowMind .NET  
**Backend da plataforma FlowMind — monitoramento de equilíbrio mental e bem-estar diário**

---

# Integrantes  
| Nomes | RM |
|------|----|
| Sofia Domingues Gonçalves | 554920 |
| Júlia Soares Farias dos Santos | 554609 |

---

# Sobre o Projeto  
O **FlowMind Backend** é uma API RESTful para registro diário de **humor, energia e sono**, com cálculo de **índice de equilíbrio** e suporte a **duas versões da API**:

- **v1**: Persistência real com **Oracle + Entity Framework Core**  
- **v2**: Dados em memória + **HATEOAS**, **índice de equilíbrio** e **paginação**

A plataforma permite que usuários façam **check-ins diários** e monitorem seu bem-estar com **boas práticas REST**, **observabilidade** e **testes automatizados**.

---

# Funcionalidades  

- **Check-in diário** com humor, energia e sono  
- **Cálculo do Índice de Equilíbrio** (0 a 100)  
- **Paginação completa** (`page`, `pageSize`, `total`, `totalPages`)  
- **HATEOAS** com links `self`, `next`, `previous`, `create`, `update`, `delete`  
- **Versionamento de API** (`/api/v1` e `/api/v2`)  
- **Health Check**, **Logging** e **Tracing** com OpenTelemetry  
- **Persistência com Oracle + EF Core + Migrations**  
- **Testes de integração com xUnit**  

---

# Estrutura do Projeto  

```FlowMind/
├── Flowmind/                 
├── Flowmind.Tests/       
├── Flowmind.csproj
├── README.md
└── appsettings.json
```


---

# Rotas da API  

### Versão 1.0 — Banco Real (Oracle)  
> Base URL: `/api/users/{idUsuario}/checkins`

| Método | Rota | Descrição | Corpo da Requisição | Resposta Esperada |
|--------|------|-----------|----------------------|-------------------|
| `GET` | `/api/users/1/checkins?page=1&pageSize=5` | Lista check-ins com paginação | — | `200 OK` + `totalPages`, `items` |
| `GET` | `/api/users/1/checkins/{id}` | Busca um check-in | — | `200 OK` ou `404 Not Found` |
| `POST` | `/api/users/1/checkins` | Cria check-in (1x por dia) | ```json { "humor": "Calmo", "energia": "Alta", "sono": "Bom" } ``` | `201 Created` |
| `PUT` | `/api/users/1/checkins/{id}` | Atualiza check-in | Mesmo do POST | `204 No Content` |
| `DELETE` | `/api/users/1/checkins/{id}` | Remove check-in | — | `204 No Content` |

---

### Versão 2.0 — In-Memory + HATEOAS + Índice  
> Base URL: `/api/v2/checkin`

| Método | Rota | Descrição | Corpo da Requisição | Resposta Esperada |
|--------|------|-----------|----------------------|-------------------|
| `GET` | `/api/v2/checkin?page=1&pageSize=10` | Lista com HATEOAS e índice | — | `200 OK` + `links`, `indiceEquilibrio` |
| `GET` | `/api/v2/checkin/{id}` | Busca com HATEOAS | — | `200 OK` + `links` |
| `POST` | `/api/v2/checkin` | Cria check-in | ```json { "humor": "Motivado", "energia": "Alta", "sono": "Bom" } ``` | `201 Created` + `Location` |
| `PUT` | `/api/v2/checkin/{id}` | Atualiza | Mesmo do POST | `200 OK` |
| `DELETE` | `/api/v2/checkin/{id}` | Remove | — | `204 No Content` |

---

# Entidades e DTOs  

### `CheckinDiario` (Entidade)  
```
public class CheckinDiario
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Data { get; set; }
    public Humor Humor { get; set; }
    public Energia Energia { get; set; }
    public Sono Sono { get; set; }
}
```

### Enums
``` 
public enum Humor { Ansioso, Neutro, Motivado, Estressado, Calmo }
public enum Energia { Baixa, Media, Alta }
public enum Sono { Ruim, Ok, Bom }
```
## CheckinResponse (DTO)

```
public class CheckinResponse
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public Humor Humor { get; set; }
    public Energia Energia { get; set; }
    public Sono Sono { get; set; }
    public List<LinkDto> Links { get; set; } = new();
} 
```


# Tecnologias Utilizadas


| Tecnologia | Finalidade | 
|--------|------|
| C# (.NET 8) | Linguagem e framework principal | 
| ASP.NET Core | API RESTful | 
| Entity Framework Core | ORM + Migrations | 
| Oracle | Banco de dados relacional | 
| AutoMapper | Mapeamento de objetos | 
| Swashbuckle (Swagger) | Documentação interativa | 
| OpenTelemetry | Tracing (console) | 
| xUnit | Testes de integração |




# Como Rodar o Projeto

## 1. Clone o repositório
```
bashgit clone https://github.com/SEU_USUARIO/FlowMind.git
cd FlowMind
```

## 2. Restaure dependências
```
bashdotnet restore
```

## 3. Configure o Oracle
```
Use user-secrets ou variável de ambiente (nunca commite senhas!)
bash# PowerShell
setx ConnectionStrings__OracleConnection "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=RM554920;Password=SUA_SENHA"

# Ou com user-secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "Data Source=...;User Id=RM554920;Password=..."
```

## 4. Execute as migrations
```
bashdotnet ef database update --project Flowmind
```

## 5. Execute a API
```
bashdotnet run --project Flowmind
```

## 6. Acesse o Swagger
```
v1: https://localhost:5001/swagger/v1/swagger.json
v2: https://localhost:5001/swagger/v2/swagger.json
```
# Monitoramento
## Health Check
```
GET /health
```
- Retorna 200 OK se Oracle estiver saudável
## Logging

- Console + Debug (desenvolvimento)

## Tracing

- OpenTelemetry com exportador para console


# Testes Automatizados
```
cd Flowmind.Tests
dotnet test
```
### Saída esperada:
```
textTest Run Successful.
Total tests: 3
     Passed: 3
```
### Cobertura:

- v1: Paginação (totalPages)
- v2: HATEOAS (links) + Índice (indiceEquilibrio)
- POST: Status 201 Created + Location header

---
# Versionamento da API

| Versão | Rota Base                  | Banco      | Recursos                              |
|--------|----------------------------|------------|---------------------------------------|
| **v1** | `/api/users/{id}/checkins` | Oracle     | Paginação, CRUD                       |
| **v2** | `/api/v2/checkin`          | In-memory  | HATEOAS, Índice de Equilíbrio, Paginação |