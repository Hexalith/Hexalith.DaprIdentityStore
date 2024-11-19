# Hexalith.DaprIdentityStore

Dapr Actor implementation of Microsoft Identity Store - a distributed, scalable identity management solution built on Dapr actors.

## Build status

[![Build status](https://github.com/Hexalith/Hexalith.DaprIdentityStore/actions/workflows/hexalith_build.yml/badge.svg)](https://github.com/Hexalith/Hexalith.DaprIdentityStore/actions)
[![NuGet](https://img.shields.io/nuget/v/Hexalith.DaprIdentityStore.Server.svg)](https://www.nuget.org/packages/Hexalith.DaprIdentityStore.Server)
[![License: MIT](https://img.shields.io/github/license/hexalith/hexalith.Security)](https://github.com/hexalith/hexalith.Security/blob/main/LICENSE)
[![Discord](https://img.shields.io/discord/1063152441819942922?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discordapp.com/channels/1102166958918610994/1102166958918610997)
  
[![Coverity Scan Build Status](https://scan.coverity.com/projects/30308/badge.svg)](https://scan.coverity.com/projects/hexalith-hexalith-Security)
  
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/9f3644b4447a401189fcbd10738dd964)](https://app.codacy.com/gh/Hexalith/Hexalith.DaprIdentityStore/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
  
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Hexalith_Hexalith.DaprIdentityStore&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Hexalith_Hexalith.DaprIdentityStore)

## Overview

Hexalith.DaprIdentityStore provides a distributed implementation of ASP.NET Core Identity storage using Dapr actors. This approach offers high scalability, resilience, and consistency for managing user identities across distributed applications.

## Features

- **Distributed Identity Storage**: Leverages Dapr actors for reliable, distributed identity management
- **ASP.NET Core Identity Compatible**: Implements standard Identity interfaces for seamless integration
- **Scalable Architecture**: Built on Dapr's actor model for horizontal scalability
- **High Availability**: Supports distributed deployment scenarios
- **Consistent Data**: Actor-based state management ensures data consistency
- **Cloud-Native**: Designed for modern cloud environments
- **Platform Agnostic**: Works across different cloud providers and on-premises

## Getting Started

### Prerequisites

- .NET 8.0 or later
- Dapr runtime installed
- Visual Studio 2022 or later (recommended)

### Installation

Install via NuGet Package Manager

## Packages

- [Hexalith.DaprIdentityStore](Hexalith.DaprIdentityStore/README.md)
