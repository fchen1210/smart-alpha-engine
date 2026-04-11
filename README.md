# Smart Alpha Engine

AI-driven portfolio and market analysis engine designed to monitor risk, analyze market conditions, and generate personalized investment insights.

---

## Vision

Smart Alpha Engine aims to become an intelligent decision-support system for investors. Instead of reacting to markets blindly, this engine helps users:

- Understand market conditions
- Evaluate portfolio risk
- Detect concentration and exposure risks
- Generate structured daily insights
- Suggest allocation adjustments based on risk profile

The long-term vision is to evolve into an AI-powered portfolio intelligence platform — with **Human in the Loop** as a core design principle. The engine provides analysis and recommendations; final decisions always rest with the investor.

---

## Architecture Overview

The system is organized into 6 layers, from raw user input to final human decision.

```
Users (Portfolio Holdings)
        │
        ▼
C# Data Engine  ─── Yahoo Finance · Alpha Vantage · SEC EDGAR · Polygon.io
        │
        ▼
Data Buckets
  ├── Technical Indicators  (RSI · MACD · Bollinger Bands)
  ├── Market Data           (S&P 500 · Nasdaq · VIX)
  ├── Fundamentals / Raw    (10-K · 10-Q · PE/PB)
  └── Macro Sentiment       (Gold · BTC · TNX)
        │
        ▼  ─── Claude / GPT API ───
6 AI Agents
  Row A (parallel):
  ├── Technical Analysis Agent  — Trends · Signals · Support & Resistance
  ├── Portfolio Mgmt Agent      — Positions · Weights · Concentration
  ├── Risk Mgmt Agent           — Drawdown · Beta · Risk Alerts
  └── Market Feeling Agent      — Macro Sentiment · Safe-Haven Signals  (optional / future)

  Row B (synthesizing):
  ├── Accounting Agent          — P&L · Return Rate · Cost Basis · Per-Portfolio Calculation
  └── Value Calculation Agent   — DCF · PE/PB · Intrinsic Value · Multiple Valuation Models
        │
        ▼
Consolidated Report
  Daily closing report + intraday real-time alerts
        │
        ▼
You — Final Decision  (Human in the Loop)
```

---

---

## Runtime Flow — Daily Execution Lifecycle

This section describes how the system operates during a normal daily cycle.

**Example Daily Execution**

08:00 — Fetch market data from external providers  
08:02 — Normalize raw datasets into unified schema  
08:03 — Compute technical indicators and macro metrics  
08:05 — Run Market Regime Detection  
08:06 — Analyze portfolio structure and exposure  
08:07 — Compute risk metrics  
08:08 — Generate consolidated daily report  
08:09 — Save report snapshot and trigger alerts if thresholds exceeded  

**Execution Modes**

- **Daily Mode** — End-of-day full analysis (primary mode)
- **Intraday Mode** — Lightweight risk monitoring (future)
- **Backtest Mode** — Historical replay analysis (future)


## AI Agent Details
---

## Agent Priority Roadmap

All six agents are part of the long-term architecture.  
However, implementation follows strict execution priority.

---

## System Maturity Levels

The system evolves through clearly defined maturity stages.

### Level 1 — Functional

- Market data ingestion works
- Portfolio can be loaded
- Basic risk metrics calculated
- Daily report generated

**Target:** First usable system

---

### Level 2 — Reliable

- Full test coverage for risk metrics
- Stable daily execution
- Consistent historical outputs

**Target:** Trustworthy system

---

### Level 3 — Intelligent

- Regime detection models
- Risk scoring system
- Multi-portfolio support

**Target:** Insightful system

---

### Level 4 — Scalable

- Cloud deployment
- Dashboard interface
- Multi-user capability

**Target:** Production-ready system

### Phase 1 — Core Agents (MVP Priority)

These agents are implemented first and form the operational backbone.

1. **Portfolio Mgmt Agent**
   - Position tracking
   - Portfolio structure analysis
   - Exposure measurement

2. **Risk Mgmt Agent**
   - Drawdown calculation
   - Beta estimation
   - Risk alert triggers

3. **Market Regime Agent**
   - Risk-On / Risk-Off classification
   - Volatility regime detection
   - Macro stress signals

These three agents enable the first functional daily report.

---

### Phase 2 — Supporting Agents

4. **Accounting Agent**
   - P&L calculation
   - Cost basis tracking

5. **Technical Analysis Agent**
   - Trend identification
   - Signal detection

---

### Phase 3 — Advanced Intelligence

6. **Value Calculation Agent**
   - Intrinsic value models
   - Multi-model valuation

7. **Market Feeling Agent** *(optional / future)*
   - Macro sentiment modeling

### Row A — Parallel Analysis

| Agent | Responsibility | Data Sources |
|---|---|---|
| **Technical Analysis Agent** | Identify trends, trading signals, support & resistance levels | Technical indicators (RSI · MACD · Bollinger Bands) |
| **Portfolio Mgmt Agent** | Track multiple portfolios, analyze position weights and concentration | User holdings · Market data |
| **Risk Mgmt Agent** | Calculate drawdown and Beta, trigger risk alerts | Market data · Holdings data |
| **Market Feeling Agent** | Read macro sentiment, identify safe-haven signals *(optional)* | Gold · BTC · TNX · VIX |

### Row B — Synthesis

| Agent | Responsibility | Data Sources |
|---|---|---|
| **Accounting Agent** | Calculate P&L, return rate, and cost basis independently per portfolio | Holdings data · Portfolio Mgmt Agent |
| **Value Calculation Agent** | DCF, PE/PB, Graham formula — multi-model intrinsic value estimation | Raw fundamentals (10-K · 10-Q) |

---

## Data Sources (C# Engine)

The C# data engine is responsible for fetching, normalizing, and distributing market data to all agents.

| Source | Data Type |
|---|---|
| Yahoo Finance | Stock/ETF prices, historical data |
| Alpha Vantage | Technical indicators, real-time quotes |
| SEC EDGAR | 10-K and 10-Q filings |
| Polygon.io | Real-time market data, market depth |

---

## Core Objectives

The project focuses on four primary capabilities:

**1. Market Data Aggregation** — Collect structured market data: stock/ETF prices, volatility (VIX), interest rates, FX, commodities, and macro indicators.

**2. Market State Analysis** — Transform raw data into interpretable signals: Risk On/Off detection, volatility regime classification, rate pressure, dollar strength/weakness.

**3. Portfolio Risk Analysis** — Analyze portfolio structure: risk level, concentration, drawdown sensitivity, exposure imbalance, and risk alerts.

**4. Personalized Investment Insights** — Generate structured commentary: daily portfolio commentary, risk warnings, and allocation suggestions.

---

---

## Core Risk Metrics

The system evaluates portfolio safety using structured risk indicators.

### Portfolio-Level Metrics

- Max Drawdown
- Rolling Drawdown
- Portfolio Beta vs SPY
- Portfolio Volatility
- Cash Allocation Ratio
- Risk Score (composite metric)

### Exposure Metrics

- Sector Concentration
- Single Position Risk
- Technology Sector Exposure
- Geographic Exposure (future)
- Asset-Class Exposure

### Stress Sensitivity

- Drawdown Sensitivity to Market Drops
- Volatility Shock Sensitivity
- Rate Shock Sensitivity (future)

## Output: Daily Report

The consolidated report combines outputs from all 6 agents into:

- **Daily Closing Report** — End-of-day summary covering market state, portfolio performance, risk flags, and valuation signals
- **Intraday Real-Time Alerts** — Triggered alerts for significant risk events or threshold breaches

---

## MVP Scope (Phase 1)

Focused on daily risk intelligence, not trading automation.

---

## Non-Goals (Phase 1)

To maintain focus, the following features are intentionally excluded:

- Automated trading execution
- Broker API integration
- High-frequency trading
- Leverage management
- Short-term price prediction
- Market timing claims
- Complex derivatives modeling

Phase 1 focuses strictly on **risk intelligence**, not trading automation.

**Included:**
- Daily market data ingestion
- Basic market regime detection
- Portfolio exposure analysis
- Risk summary generation
- Text-based daily report

**Not included (yet):**
- Real-time trading / order execution
- High-frequency data
- Broker integration

---

## Tech Stack

| Layer | Technology |
|---|---|
| Data Engine | C# (.NET) |
| AI Agents | Claude API / GPT API |
| Market Data | REST APIs (Yahoo Finance, Alpha Vantage, SEC EDGAR, Polygon.io) |
| Configuration | JSON-based config |
| Scheduling | Daily scheduled jobs |
| Future | Python (data science), Azure / cloud, LLM-generated commentary |

---

## Repository Structure (Planned)

```
smart-alpha-engine/
├── src/
│   ├── SmartAlpha.Core
│   ├── SmartAlpha.Data
│   ├── SmartAlpha.Analytics
│   ├── SmartAlpha.Agents
│   ├── SmartAlpha.Reporting
│   └── SmartAlpha.API
├── tests/
│   └── SmartAlpha.Tests
├── docs/
│   ├── architecture.md
│   └── roadmap.md
└── scripts/
    ├── data-fetch
    └── daily-run
```

---

## Roadmap

**Phase 1 — Foundation**
- Market data ingestion (C# engine)
- Portfolio model & holdings loader
- Basic risk metrics (drawdown · Beta · concentration)
- Technical Analysis / Portfolio Mgmt / Risk Mgmt Agents
- Daily report engine

**Phase 2 — Intelligence**
- Accounting Agent (P&L, cost basis per portfolio)
- Value Calculation Agent (DCF, PE/PB, Graham)
- Regime detection & risk scoring models
- Multi-asset support
- Web API

**Phase 3 — Interface**
- Dashboard UI
- Historical tracking & visualization
- Intraday alert system

**Phase 4 — AI Enhancement**
- Market Feeling Agent (macro sentiment / safe-haven signals)
- LLM-generated personalized insights
- Scenario simulation
- Per-user investment style detection

---
---

## Portfolio Input Schema

Portfolios are provided as structured JSON files.


**Example:**

```json
{
  "portfolioName": "Main Portfolio",
  "baseCurrency": "USD",
  "positions": [
    {
      "symbol": "SPY",
      "weight": 0.40
    },
    {
      "symbol": "QQQ",
      "weight": 0.25
    },
    {
      "symbol": "NVDA",
      "weight": 0.15
    },
    {
      "symbol": "CASH",
      "weight": 0.20
    }
  ]
}
```

## Example Use Case

A user provides two portfolios:

- **Portfolio 1:** SPY 40%, QQQ 25%, NVDA 15%, Cash 20%
- **Portfolio 2:** AAPL 30%, MSFT 30%, Cash 40%


Engine generates:

> **Market State:** Risk-Off Transition Detected  
> **Portfolio 1 Risk:** High concentration in tech sector — sensitive to volatility spikes  
> **Portfolio 2 Risk:** Low diversification — single-sector exposure  
> **Valuation:** NVDA trading above DCF intrinsic value by ~35%  
> **Suggested Action:** Reduce single-stock exposure; increase defensive allocation

Final decision: **You.**

---

## Design Principles

- Modular, agent-based architecture
- Clear separation of concerns between data, analysis, and intelligence layers
- Human in the Loop — AI advises, humans decide
- Testable components with config-driven behavior
- Extensible data sources and agent plugins

---

## Status

Early-stage development. Architecture defined (v2). Core C# data engine and agent framework in progress.

---

## License

To be defined.
