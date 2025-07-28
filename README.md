## 📦 **Simple Product Catalog & Price Microservices**

> 🧪 A small learning project exploring how two independent ASP.NET Core Web API projects can communicate using `HttpClient`.

---

### ✏ **Description**

This is a beginner-friendly project built with ASP.NET Core where:
- One API manages **products** (their IDs and names).
- Another API manages **prices** for those products.
- The two services talk to each other using **HttpClient**, which was my focus in this project.
- Includes a simple rollback mechanism: if price creation fails, the product creation also rolls back.

The aim wasn’t to build something complex, but to practice how real microservices might coordinate between each other, even in small scenarios.

---

### 📡 **Why this project matters (even if small)**

✅ Practiced designing two separate ASP.NET Core APIs.  
✅ Used `HttpClient` to let them communicate (instead of putting everything in one project).  
✅ Implemented basic CRUD operations on both services:
- Create, Read, Update, Delete products.
- Create, Read, Update, Delete prices.
✅ Added a simple rollback: if the price service call fails when creating a product, the product creation also fails to keep data consistent.

---

### 🛠 **Technologies used**

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server (localdb)
- Swagger / OpenAPI
- C#
- HttpClient

---

### 🚀 **Future improvements / ideas**

- Add proper **Distributed Transaction** (using libraries like `TransactionScope` or message queues) instead of simple rollback.
- Implement retry policies for handling price service downtime.
- Add authentication & authorization.
- Add logging & monitoring.
- Write integration tests covering the HttpClient calls between services.

---

### 🤓 **About**

I built this project mainly to learn:
- How two APIs can call each other using `HttpClient`.
- How to handle simple coordination and rollback between services.

I know it’s a small project, but it helped me a lot to see how microservices *could* work in real life.

---

## ⭐ **Usage**

You can run both APIs locally (e.g., `https://localhost:xxxx` for product API and another port for price API), then use Swagger UI to test:
- `POST /api/Products` → creates a product & calls price API to add its price.
- `GET /api/Products` → list products with prices.
- `PUT` & `DELETE` endpoints to update product name / price or remove them.

---

*Thanks for reading! Feel free to fork or suggest ideas.* 😊
