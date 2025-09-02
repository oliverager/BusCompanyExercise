# 🚌 Bus Company Pricing Exercise

A simple C# (.NET 9) school exercise implementing a **bus trip price calculator**.

## 📋 Pricing Rules
- Initial fee: **2500 kr**
- First 100 km: **10 kr/km**
- Next 400 km (100–500 km): **8 kr/km**
- Beyond 500 km: **6 kr/km**

## 🧪 Features
- Clean calculator implementation in C#
- **Unit tests** using xUnit + FluentAssertions
- Covers boundary cases (0 km, 100 km, 500 km, fractional km, etc.)
- Demonstrates a **TDD approach**

## ▶️ Run the project
```bash
# Restore dependencies
dotnet restore

# Run tests
dotnet test
