Para garantir que o usuário possa visualizar o background da página quando o popup de "Li e Aceito" está aberto é necessário adicionar uma webpart de conteúdo e nela adicionar o código HTML abaixo.

<style type="text/css>
.ms-dlgOverlay
{
    position:static !important;
    filter:alpha(opacity=30) !important;
    opacity:0.3 !important;
    background-color: #e5ecf4 !important;
}
</style>
