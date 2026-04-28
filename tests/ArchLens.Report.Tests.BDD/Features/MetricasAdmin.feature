# language: pt-BR
Funcionalidade: Métricas administrativas de Relatórios
  Como um administrador da plataforma ArchLens
  Eu quero consultar as métricas de relatórios
  Para monitorar a qualidade das análises de arquitetura

  Cenário: Consultar métricas com role Admin
    Dado que eu sou um usuário autenticado com role "Admin"
    E que existem métricas de relatórios disponíveis
    Quando eu consultar as métricas administrativas de relatórios
    Então a resposta deve ter status code 200
    E a resposta deve conter as métricas de relatórios

  Cenário: Consultar métricas com role User
    Dado que eu sou um usuário autenticado com role "User"
    E que existem métricas de relatórios disponíveis
    Quando eu consultar as métricas administrativas de relatórios
    Então a resposta deve ter status code 200

  Cenário: Consultar métricas sem autenticação
    Dado que eu não estou autenticado
    E que existem métricas de relatórios disponíveis
    Quando eu consultar as métricas administrativas de relatórios
    Então a resposta deve ter status code 401

  Cenário: Métricas retornam score médio correto
    Dado que eu sou um usuário autenticado com role "Admin"
    E que existem métricas com score médio de 8.5
    Quando eu consultar as métricas administrativas de relatórios
    Então a resposta deve ter status code 200
    E a resposta deve conter score médio de 8.5

  Cenário: Métricas retornam uso de providers
    Dado que eu sou um usuário autenticado com role "Admin"
    E que existem métricas de relatórios disponíveis
    Quando eu consultar as métricas administrativas de relatórios
    Então a resposta deve ter status code 200
    E a resposta deve conter uso de providers
