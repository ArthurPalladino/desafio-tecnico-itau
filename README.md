# ItauTopFive - Sistema de Compra Programada de Ações

ItauTopFive é um motor de investimentos automatizado desenvolvido para gerenciar o ciclo de vida de aportes na carteira recomendada Itaú Top 5. O sistema processa aportes mensais, realiza a compra proporcional de ativos e automatiza o cálculo de tributos e rebalanceamento de custódia.

## Tecnologias Utilizadas

- **Linguagem**: C# (.NET 8.0)
- **ORM**: Entity Framework Core (EF Core)
- **Banco de Dados**: MySQL
- **Documentação API**: Swagger/OpenAPI
- **Mensageria**: Apache Kafka
- **Testes & Qualidade**: xUnit, Moq

## Arquitetura

O projeto foi desenvolvido utilizando a **Layered Architecture** (Arquitetura em Camadas), garantindo a separação de responsabilidades entre os componentes:

- **Api**: Camada de entrada, responsável pelos Controllers REST e configuração de Injeção de Dependência.
- **Services**: Camada de lógica de negócio, contendo os motores de execução (PurchaseEngine e Rebalancing) e lógica tributária.
- **Repositories**: Camada de abstração de dados, responsável pela comunicação com o banco de dados via EF Core.
- **Entities**: Contém as entidades de negócio, as regras fundamentais do sistema e os DTOs.
- **Data**: Contexto do banco de dados (DbContext) e configurações de mapeamento do ORM.

## Funcionalidades

- **Motor de Compra Programada**: Motor inteligente que processa o capital enviado pelo investidor e executa a compra de ativos de forma autônoma.
- **Distribuição Proporcional**: Alocação de ativos baseada nos pesos da carteira recomendada (Top Five).
- **Rebalanceamento Automático**: Ajuste de custódia por mudança de recomendação ou desvios de carteira.
- **Cálculo de Impostos**: Subsistema que provisiona automaticamente a retenção de IR (Dedo-Duro) e apura o lucro real para cálculo de imposto sobre ganho de capital.
- **Parser B3**: Parser para o arquivo `COTAHIST` da B3 para atualização de preços e ativos.

## Pré-requisitos

- [.NET 8.0](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0) 
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)

## 🛠️ Como Executar

1. **Subir containers docker (MySQL + Kafka)**:
   ```bash
   docker-compose up -d
   ```

2. **Rodar o projeto**:
   ```bash
   dotnet restore
   dotnet run --project src/ItauTopFive.csproj
   ```
   Abra o arquivo dashboard.html para usar a dashboard
   ou navegue até https://localhost:7239/swagger/index.html

3. **Rodar testes**:
   ```bash
   dotnet test tests/ItauTopFive.Tests.csproj
   ```
   
## 📁 Estrutura de Pastas

```text
/
|-- cotacoes/                
|-- documents/               
|-- src/                     
|   |-- Controllers/        
|   |-- Data/                
|   |-- Entities/            
|   |-- Exceptions/          
|   |-- Messaging/           
|   |-- Middleware/         
|   |-- Migrations/          
|   |-- Repositories/        
|   |-- Services/            
|-- tests/                   
|   |-- Controllers.Tests/   
|   |-- Entities.Tests/      
|   |-- Repositories.Tests/  
|   |-- Services.Tests/      
|-- docker-compose.yml       
|-- ItauTopFive.sln              
```
-------------------------------------

## 📈 Qualidade e Testes

A solução foi submetida a uma bateria de testes focada na **lógica de negócio** e **integridade das regras financeiras** (excluindo infraestrutura como *migrations*, *DTOs* ou configurações de framework).

### Cobertura atual de testes

- **Line Coverage:** 84.1%
- **Branch Coverage:** 73.7%
- **Method Coverage:** 85.0%

---

Desenvolvido como parte do **processo seletivo para Engenheiro de Software Jr**.


