# Update
- 2023-5-31 Add the code

## About IdentityServer4 Admin

Based on [IdentityServer4](https://github.com/IdentityServer/IdentityServer4)， this project is used to provide a web UI form manage the settings for IdentityServer 4.

## Source code structure

- +server : based on webapi , freesql, csredis 
  - IdentityServer4.Admin.WebApi 
  - IdentityServer4.Storage.FreeSql
  - IdentityServer.QuickStart.UI
- +admin  : Web UI for manage the IdentityServer4, based on VUE, Antd-VUE

## How to use

### Develop
Put your local mysql and redis config to project secrets. 

### Build

Build the webapi and the view into single image

```bash
docker build -t identity-admin:localest -f ./docker/Dockerfile .
```
### Demo Site

[http://demo-idsadm.dotnetcore.icu/admin/](http://demo-idsadm.dotnetcore.icu/admin/) 