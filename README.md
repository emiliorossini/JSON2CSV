# JSON2CSV - JSON → CSV Converter

**Feito por Emilio Nicolau Rossini**

Uma ferramenta completa, robusta e com design premium para converter arrays de objetos JSON em CSV. Construída com as tecnologias mais modernas do ecossistema .NET.

### Funcionalidades
- Conversão instantânea de array de objetos JSON → CSV  
- Mantém exatamente a ordem dos campos do primeiro objeto  
- Validação completa com mensagens claras e amigáveis  
- Bloqueio total de:  
  - JSON vazio ou malformado (com indicação da linha do erro)  
  - Objetos vazios ou só com `null`/strings vazias  
  - Estruturas aninhadas (objetos ou arrays dentro de objetos)  
  - Arquivos muito grandes (> 500 KB) ou com mais de 10.000 linhas  
- Pré-visualização em tabela interativa  
- Botão "Copiar CSV" direto para a área de transferência  
- Botão "Limpar tudo"  
- Feedback visual imediato com alertas de sucesso e erro com ícones  

### Tecnologias utilizadas

![.NET 9](https://img.shields.io/badge/.NET-9.0-5C2D91.svg?style=for-the-badge&logo=.NET)  
![Blazor](https://img.shields.io/badge/Blazor-512BD4.svg?style=for-the-badge&logo=blazor)  
![MudBlazor](https://img.shields.io/badge/MudBlazor-512BD4.svg?style=for-the-badge&logo=mudblazor&logoColor=white)
