# Smart Alpha Engine Architecture

## Modules

SmartAlpha.API
SmartAlpha.Domain
SmartAlpha.Data
SmartAlpha.Analytics
SmartAlpha.Reporting

## Dependency Rules

API → Reporting → Analytics → Domain

Data → Domain

Domain does NOT depend on Data

Analytics depends on Domain

Reporting depends on Analytics

## Principles

- Modular Monolith
- Domain-first design
- Replaceable infrastructure
- AI-ready design
