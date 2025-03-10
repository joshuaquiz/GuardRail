# GaurdRail

GuardRail is an open source .Net core project used for physical access control.

Build status: ![.NET Core](https://github.com/joshuaquiz/GuardRail/workflows/.NET%20Core/badge.svg?branch=develop)

## Structure

* Core - Where core/common/shared helpers are placed. These items should be a DTO and functional as possible.
* Core.Models - Where core/common/shared models, enums, and exceptions are placed. These items should be a DTO and functional as possible.

* Api.Main - The main hosted API. Used for cloud management.

* Database.Main - The main database and it's bindings. The raw Models should be in Core.Models.

* Logic - Where the high level logic classes exist.

* Local.Service - The local service that will be installed.

## Supported hardware:

