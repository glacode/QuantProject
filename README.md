
# QuantProject

**QuantProject** is a modular and extensible C# framework for developing, backtesting, and deploying algorithmic trading strategies. Originally developed over 11+ years (first hosted on SourceForge), this system provides a solid foundation for quantitative finance research and trading automation.

## 🚀 Features

- ⚙️ **Strategy Framework**: Define and compose strategies with plug-in architecture.
- 📊 **Backtesting Engine**: Simulate strategies on historical data with realistic slippage and order execution modeling.
- ⏱️ **Live Trading Support**: Interfaces for real-time data and execution (can be customized).
- 📁 **Modular Design**: Organized into layers like Business Logic, Data Access, Execution, and UI.
- 🧪 **Strategy Examples**: Includes implementations like `BasicStrategyForBacktester` and templates for rapid development.
- 🔍 **Debug and Trace Tools**: Custom logging, tracing, and performance measurement utilities.
- 📥 **QuantDownloader Tool**: A standalone downloader to fetch historical data for backtesting.

## 🧱 Project Structure

```
QuantProject/
├── b1_Environment/           # Core interfaces, configuration, and logging
├── b2_DataAccess/            # Market data input and storage
├── b3_Model/                 # Data structures and models
├── b4_Business/              # Strategies and execution logic
├── b5_View/                  # (Optional) Visualization or UI components
├── Utilities/                # Shared helper functions and extensions
└── QuantDownloader/          # Tool to download historical market data
```

## 🛠️ Getting Started

### Requirements

- [.NET Framework 4.0 or higher](https://dotnet.microsoft.com/)
- Visual Studio (or compatible C# IDE)

### Build Instructions

```bash
git clone https://github.com/glacode/QuantProject.git
# Open the QuantProject.sln file in Visual Studio and build
```

### Run a Sample Backtest

1. Navigate to `b4_Business/a2_Strategies/BasicStrategyForBacktester.cs`.
2. Modify the strategy or parameters.
3. Launch the backtester application or call it via `Main()`.

## 📥 QuantDownloader

QuantDownloader is a utility project included in this repository that allows users to fetch and store historical market data for use in backtesting and live trading. It supports:

- Configurable instruments and date ranges
- Support for various data providers (can be extended)
- Integration with QuantProject’s expected data format

To run:

```bash
cd QuantDownloader
# Build and run via Visual Studio or command line
```

This tool should be run before any backtests or simulations to ensure up-to-date data is available.

## 📸 Screenshots

<!-- If you have any screenshots of the UI or logs, insert here -->
<!-- ![Example Screenshot](screenshots/backtest_result.png) -->

## 📦 Packaging

The project is currently organized as multiple class libraries and executables. You can bundle components into a standalone trading or simulation app.

## 🧠 Philosophy

QuantProject is designed to **separate concerns**, enable **reuse of trading logic**, and provide **control over every layer**, from data access to execution. No magic black-boxes — everything is hackable.

## 📚 History

Originally started on in 2003, this codebase has evolved through years of iterative improvements and real-world trading experiments. Now open-sourced on GitHub for archival, educational, and collaborative purposes.

## 📄 License

[MIT License](LICENSE)

## 🙏 Acknowledgments

- Inspired by research in algorithmic trading and portfolio theory.
- Special thanks to the early collaborators and testers over the years.

---

Feel free to explore, fork, or contribute!
