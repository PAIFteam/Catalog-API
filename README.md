# Catalog API

> Microsserviço responsável pelo **catálogo de produtos/jogos** da plataforma **PAIF Games** e pela **orquestração do fluxo de compra** via eventos. Atua como **publisher e consumer** em uma arquitetura **event-driven**, integrando Catalog → Payments → Notifications.

---

## 🎯 Objetivo

Este serviço existe para:

* Manter o **catálogo de produtos** (CRUD de produtos/jogos)
* Receber requisições para associar um jogo ao usuário (fluxo de compra)
* Publicar evento de **pedido criado** (`OrderPlacedMessage`)
* Consumir evento de **pagamento processado** (`PaymentProcessedMessage`)
* Efetivar a compra gravando a relação **Game ↔ User** no banco

---

## 🧱 Arquitetura e Tecnologias

* .NET 8
* Minimal APIs
* Carter (módulos de rotas)
* CQRS (Commands / Queries)
* MediatR
* Dapper (SQL Server)
* MassTransit
* RabbitMQ
* PostgreSQL (document store para catálogo)
* Docker (multi-stage build)

Arquitetura em camadas:

* API (Endpoints)
* Core (Domain + Application / UseCases)
* Infra (Data + Messaging)
* BuildingBlocks (abstrações CQRS)

---

## 📦 Responsabilidades do Serviço

* CRUD de produtos do catálogo
* Iniciar fluxo de compra/publicação de pedido
* Consumir resultado do pagamento
* Efetivar compra (persistência no banco relacional)

> Decisão de design: **Catalog dispara o evento e espera o resultado via mensageria**, sem acoplamento direto com Payments.

---

## 📡 Mensageria (RabbitMQ)

### 🔹 Evento Publicado

| Evento               | Quando publica                                 | Fila                 |
| -------------------- | ---------------------------------------------- | -------------------- |
| `OrderPlacedMessage` | Ao iniciar a compra (ex: `PutGameUserUseCase`) | `order_placed_queue` |

### 🔹 Evento Consumido

| Evento                    | Origem   | Fila                      |
| ------------------------- | -------- | ------------------------- |
| `PaymentProcessedMessage` | Payments | `payment_processed_queue` |

---

## 🔄 Fluxo de Integração (Compra)

1. Cliente chama endpoint de compra (ex: `GET /PutGameUser`)
2. Catalog publica `OrderPlacedMessage` no RabbitMQ
3. Payments consome, processa e publica `PaymentProcessedMessage`
4. Catalog consome `PaymentProcessedMessage`
5. Se pagamento aprovado, Catalog grava venda e itens (`sale` / `sale_item`) via Dapper
6. Notifications consome `PaymentProcessedMessage` e dispara notificação

Arquitetura limpa: cada serviço faz o seu e ninguém vira refém de HTTP sincrono.

---

## 🔌 Endpoints

### 📦 Produtos (Carter + CQRS)

| Método | Rota           | Descrição             |
| ------ | -------------- | --------------------- |
| POST   | /products      | Criar produto         |
| GET    | /products      | Listar produtos       |
| GET    | /products/{id} | Buscar produto por ID |
| PUT    | /products      | Atualizar produto     |
| DELETE | /products/{id} | Remover produto       |

### 🛒 Compra (use case)

| Método | Rota         | Descrição                                    |
| ------ | ------------ | -------------------------------------------- |
| GET    | /PutGameUser | Inicia compra e publica `OrderPlacedMessage` |

> Observação: esse endpoint está como **GET** no código atual por simplicidade/demonstração. Em produção, o ideal seria **POST**.

---

## ⚙️ Configuração

### appsettings.json (exemplo)

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5433;Database=CatalogDb;User Id=postgres;Password=***;",
    "DB_SQL_PAIF_GAMES": "Server=localhost;Database=PAIF_GAMES;User Id=***;Password=***;"
  },
  "RabbitSettings": {
    "HostName": "localhost",
    "QueueName": "order_placed_queue",
    "QueueNameConsumer": "payment_processed_queue",
    "StartConsumer": true
  }
}
```

---

## 🔐 Variáveis de Ambiente

```text
ConnectionStrings__Database
ConnectionStrings__DB_SQL_PAIF_GAMES
RabbitSettings__HostName
RabbitSettings__Username
RabbitSettings__Password
RabbitSettings__QueueName
RabbitSettings__QueueNameConsumer
RabbitSettings__StartConsumer
```

---

## 🗄️ Persistência

Este serviço usa **dois storage models** :

* **PostgreSQL** para dados do catálogo (document store)
* **SQL Server** para efetivação da venda (`sale` / `sale_item`) via **Dapper**

Isso mantém:

* Leitura do catálogo simples e rápida
* Escrita transacional de venda no banco relacional

---

## 🐳 Docker

Build:

```bash
docker build -t catalog-api -f Service/Catalog/Catalog.API/Dockerfile .
```

Run:

```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_URLS=http://+:8080 \
  catalog-api
```

---

## ▶️ Executando Localmente

Pré-requisitos:

* .NET SDK 8
* PostgreSQL
* SQL Server
* RabbitMQ
* Docker (opcional)

Run:

```bash
dotnet restore
dotnet run --project Service/Catalog/Catalog.API/Catalog.API.csproj
```

Swagger habilitado em ambiente Development.

---

## 🧠 Design Decisions

* CQRS para separar comandos e queries
* MediatR para orquestração de handlers
* RabbitMQ para desacoplamento entre serviços
* Dapper para controle fino e performance no SQL Server
* Fluxo de compra 100% event-driven (sem HTTP entre serviços)

---

## 🚫 Fora do Escopo (intencional)

* ❌ Gateway de pagamento real
* ❌ Autenticação/Autorização final (há scaffolding no código)
* ❌ Idempotência/DLQ (pode entrar no roadmap)

---

## 📄 Licença

Projeto para fins educacionais e demonstrativos.

---

**Catalog como entrypoint do fluxo de compra, com integração desacoplada e pronta para escalar.**
