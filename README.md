# Aviso De Cotação Por E-mail

## Sobre

Este projeto foi feito para um processo seletivo, onde a ideia era uma aplicação de Console, onde você passa uma ação, um valor mínimo e um máximo. E ele envia um e-mail assim que a ação atingisse um dos valores.

Eu avancei um pouco com a ideia e fiz um serviço Windows que fica checando os valores de N ações, e envia por e-mail quando alguma delas bater o valor.

E uma aplicação de Console responsável por adicionar/remover ações da lista.

A consulta do valor das ações foi feita de maneira mais genérica, permitindo facil manutenção para plugar outras APIs. Atualmente apenas tem a [AlphaVantage](https://www.alphavantage.co/support/#api-key).

## Execução

Na pasta Publish, já existe uma versão executável do projeto. Para conseguir rodar na sua máquina, você precisa editar os arquivos [appSettings.json](#appSettings) e [smtpSettings.json](#smtpSettings).

Após configurar os arquivos, basta seguir os seguintes passos:

- Copiar o appSettings e o smtpSettings para dentro da pasta Publish.

- Para adicionar/deletar uma nova ação basta executar o arquivo Publish\StockListConfig.exe e seguir as instruções na tela.

- Para executar o serviço, você precisa criar um novo serviço Windows apontando para o arquivo Publish\AvisoDeCotacaoPorEmail.exe

- Para isso, executar o seguinte no cmd:

```
sc create AvisoDeCotacaoPorEmail binPath= "<PATH>\Publish\AvisoDeCotacaoPorEmail.exe"
```

(Lembre de alterar PATH para o seu caminho)

## appSettings

O modelo do appSettings pode ser encontrado sempre no appSettings.json.sample, ou abaixo:

```json
{
  "ApiKeys": {
    "AlphaVantage": "API_KEY"
  },
  "ActiveApi": "AlphaVantage",
  "StockListCsv": "CSV_PATH",
  "TimeBetweenChecks": 900000,
  "TimeBetweenChecksIfSuccess": 1800000,
  "TimeBetweenChecksIfError": 60000,
  "DestinationEmail": "EMAIL@gmail.com",
  "NLog": {
    "targets": {
      "console": {
        "type": "Console"
      },
      "allfile": {
        "type": "File",
        "fileName": "${basedir}\\logs\\${shortdate}.log",
        "layout": "${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}"
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "allfile"
      }
    ]
  }
}
```

- ApiKeys é para armazenar as chaves das APIs usadas.

- ActiveApi é uma string escolhendo qual API está ativa.

- StockListCsv é o path completo do arquivo que será usado para guardar as ações e seus valores (o arquivo não precisa existir antes da primeira execução).

- As 3 seguintes configurações são o intervalo de tempo entre atualização do valor de cada ação (respectivamente: Entre checagens sem sucesso, com sucesso, e com erro).

  > Sucesso nesse caso é atingir o valor mínimo ou máximo.

- DestinationEmail é o E-mail de destino para os avisos.

- NLog é para as configurações do Log. Para mais informações: [NLog](https://nlog-project.org/config/)

## smtpSettings

O modelo do smtpSettings pode ser encontrado sempre no smtpSettings.json.sample, ou abaixo:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Email": "xxxxgmail.com",
    "Password": "password",
    "DisplayName": "Aviso de Cotação"
  }
}
```

Aqui ficam as configurações do servidor smtp que irá enviar o e-mail.

> No caso do Gmail, lembre-se de habilitar a configuração _Acesso a app menos seguro_ nas [configurações de segurança do Google](https://myaccount.google.com/security) (Não disponível para contas com 2FA)
