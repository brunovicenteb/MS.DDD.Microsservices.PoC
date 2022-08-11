#  Prova conceitual de arquitetura limpa para Microsserviço

Este repositório contem os projetos de dominio, Workers, API e infraestrutura da movimentação de uma prova conceitual de arquitetura limpa simplificada para Microsserviços.

[![Build-Tests](https://github.com/brunovicenteb/Microsservices.PoC/actions/workflows/Build-Test-Coverage.yml/badge.svg?branch=main)](https://github.com/brunovicenteb/Microsservices.PoC/actions/workflows/Build-Test-Coverage.yml)

<table>
  <tr>
    <th></th>
    <th>Tecnologia</th>
    <th>Versão</th>
    <th>Ferramentas</th>    
  </tr>
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/dot-net-original.svg?size=40"></td>
    <td>.Net Core</td>
    <td>6.0</td>
    <td><a href="https://serilog.net">Serilog</a>, <a href="https://xunit.net/">XUnit</a>, <a href="https://fluentvalidation.net">FluentValidation</a>, <a href="https://www.jaegertracing.io/">Jaeger</a>, <a href="https://masstransit-project.com/">MassTransit</a></td>
  </tr>
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/csharp-original.svg?size=40"></td>
    <td>C#</td>
    <td>10.0</td>
    <td></td>
  </tr>    
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/visualstudio-plain.svg?size=40"></td>
    <td>Visual Studio</td>
    <td>2022 Community</td>
    <td></td>
  </tr>    
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/git-original.svg?size=40"></td>
    <td>Git</td>
    <td>lasted</td>
    <td><a href="https://docs.github.com/pt/get-started/quickstart/github-flow">Github Flow</a></td>    
  </tr>  
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/postgresql-original.svg?size=40"></td>
    <td><a href="https://www.postgresql.org/">Postgres</a></td>
    <td>lasted</td>
    <td></td>    
  </tr> 
  <tr>
    <td><img align="center" alt="Rafa-Csharp" height="30" width="40" src="https://icongr.am/devicon/docker-original.svg?size=40"></td>
    <td><a href="https://www.docker.com/">Docker</a></td>
    <td>lasted</td>
    <td><a href="https://docs.docker.com/compose">Docker Compose</a></td>    
  </tr>
  <tr>
    <td></td>
    <td><a href="https://www.rabbitmq.com/">RabbitMQ</a></td>
    <td>lasted</td>
    <td></td>    
  </tr>  
</table>

## Pré-requisitos para a instalação do projeto:

+ Git
+ Docker
+ Docker-compose

## Instalação do projeto e configuração do ambiente:

1. Clonar o repositório:

   `
   git clone https://github.com/brunovicenteb/Microsservices.PoC.git
   `

2. Entrar no diretório criado:

   `
   cd movimentacao-service
   `

4. Subir o container:

   `
   docker-compose -p benefit up -d
   `
