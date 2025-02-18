# 🚀 Project: Missile Defense System

## **Overview**
This is a **learning project** aimed at understanding fundamental concepts in **C#**, including **asynchronous programming, TCP communication, database management, and event-driven processing**.

The **Missile Defense System** is a real-time **TCP-based server** that processes incoming missile data from clients, stores it in a **SQLite database**, and attempts interception if necessary. The system can also generate **PDF reports** summarizing missile activities.

The project consists of two main components:
1. **Main Server** (Handles incoming missile data and processes commands)
2. **Client Server** (Sends commands to the main server and retrieves data)
---
video demo: https://www.loom.com/share/cef7f4aabf5e446eb3ff96f5755cccaa?sid=687836ee-a694-45a4-b027-be624e191b53
video demo with policy: https://www.loom.com/share/4338568109db453888e36dc44e573c94 

## **🛠️ Features**

✅ **Command Processing** – Handles commands like adding missiles, fetching missile data, and generating reports.  
✅ **Database Integration** – Uses **Entity Framework Core (EF Core) with SQLite** for persistent missile tracking.  
✅ **TCP Communication** – Implements **asynchronous socket programming** to support multiple client connections.  
✅ **Missile Interception** – Detects threats and attempts to intercept incoming missiles.  
✅ **PDF Report Generation** – Uses `iTextSharp` to generate **detailed missile interception reports**.

---

## **🛠️ How It Works**

### **1. Client Sends Commands**
Clients send **commands** in the following format:
```plaintext
CommandType@Arguments
```
Example:
```plaintext
MissileInfo@Rocket,100,200,CityA
GetMissilesByCity@CityA  
```

If no arguments are needed:
```plaintext
CommandType@
```
Example:
```plaintext
GetMissileStats@
GenerateReport@
```

### **2. Server Processes the Command**
- The **Command Factory** identifies the command and determines the necessary action.
- The **Missile Repository** stores missile data in the database.
- The **Missile Interceptor** checks for threats and attempts an interception.
- The **response is sent back** to the client with the result.

### **3. Generate Reports**
A client can request a **PDF report** of missile activities:
```plaintext
GenerateReport@
```
- The server compiles **all missile data** into a **MissileReport.pdf**.
- Reports include **interception success/failure rates**, grouped by city.

---

## **🔄 Using `Task` for I/O Operations and Events**

### **Why Use `Task`?**
Since the system must handle **multiple concurrent requests**, we use **`Task` and `async/await`** to:

✅ **Prevent blocking operations** (e.g., database queries, TCP reads/writes).  
✅ **Allow multiple clients to connect simultaneously**.  
✅ **Ensure non-blocking missile interception logic**.

---

## **📦 Getting Started**

### **Prerequisites**
Ensure you have:
- **.NET 9**
- **JetBrains Rider (or Visual Studio Code)**
- **SQLite Database** (built-in with EF Core)

### **Dependencies**
Install required packages:
```sh
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package iTextSharp
```


## **🚀 Installation Guide**

### **1. Clone the Repository**
```
git clone https://github.com/Tzur-CS/MissileTrakingProject.git
```

### **💻 Setting Up the Server**

#### **2. Navigate into the Project**
```
cd MissileTraking
```

#### **3. Build the Project**
```
dotnet build
```

#### **4. Run the Server**
```
dotnet run
```

### **🛠️ Setting Up the Client**

#### **2. Navigate into the Project**
```
cd TCPServer
```

#### **3. Build the Project**
```
dotnet build
```

#### **4. Run the Client**
```
dotnet run
```

---

## **💪 Usage: Sending Commands to the Server**
### **Start the TCP Server**
The server listens on **port 5000**.

### **Client Commands**
#### **Add a Missile**
```plaintext
MissileInfo@Rocket,100,200,CityA
```
✅ Adds a missile at coordinates `(100,200)`, targeting `CityA`.

#### **Get Missiles by City**
```plaintext
GetMissilesByCity@CityA
```
✅ Retrieves missile activity for `CityA`.

#### **Generate a Report**
```plaintext
GenerateReport@
```
✅ Generates a **MissileReport.pdf** file with detailed interception statistics.

---

## **👤 Author**
📌 **Tzur Breen**  
🚀 **GitHub:** [Tzur-CS](https://github.com/Tzur-CS)

---


