# language: pt-BR
Funcionalidade: Consulta de Relatórios
  Como um usuário da plataforma ArchLens
  Eu quero consultar os relatórios de análise de arquitetura
  Para visualizar os resultados das análises realizadas

  Cenário: Consultar relatório por ID com sucesso
    Dado que eu sou um usuário autenticado com role "User"
    E que existe um relatório com ID "11111111-1111-1111-1111-111111111111"
    Quando eu consultar o relatório pelo ID "11111111-1111-1111-1111-111111111111"
    Então a resposta deve ter status code 200
    E a resposta deve conter o relatório com ID "11111111-1111-1111-1111-111111111111"

  Cenário: Consultar relatório por ID inexistente
    Dado que eu sou um usuário autenticado com role "User"
    E que não existe relatório com ID "99999999-9999-9999-9999-999999999999"
    Quando eu consultar o relatório pelo ID "99999999-9999-9999-9999-999999999999"
    Então a resposta deve ter status code 404

  Cenário: Consultar relatório por análise com sucesso
    Dado que eu sou um usuário autenticado com role "User"
    E que existe um relatório para a análise "22222222-2222-2222-2222-222222222222"
    Quando eu consultar o relatório pela análise "22222222-2222-2222-2222-222222222222"
    Então a resposta deve ter status code 200
    E a resposta deve conter a análise "22222222-2222-2222-2222-222222222222"

  Cenário: Consultar relatório por análise inexistente
    Dado que eu sou um usuário autenticado com role "User"
    E que não existe relatório para a análise "99999999-9999-9999-9999-999999999999"
    Quando eu consultar o relatório pela análise "99999999-9999-9999-9999-999999999999"
    Então a resposta deve ter status code 404

  Cenário: Listar relatórios com sucesso
    Dado que eu sou um usuário autenticado com role "User"
    E que existem relatórios cadastrados
    Quando eu listar os relatórios
    Então a resposta deve ter status code 200

  Cenário: Consultar relatório sem autenticação
    Dado que eu não estou autenticado
    E que existe um relatório com ID "11111111-1111-1111-1111-111111111111"
    Quando eu consultar o relatório pelo ID "11111111-1111-1111-1111-111111111111"
    Então a resposta deve ter status code 401

  Cenário: Listar relatórios com paginação
    Dado que eu sou um usuário autenticado com role "User"
    E que existem relatórios cadastrados
    Quando eu listar os relatórios com página 1 e tamanho 5
    Então a resposta deve ter status code 200
