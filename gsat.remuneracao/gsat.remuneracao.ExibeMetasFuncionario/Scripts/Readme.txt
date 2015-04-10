Os arquivos jquery.SPServices-0.7.2.js e jquery-1.8.3.min.js deve ser copiados para o SiteAssets de Remunerações.

Em seguida, inserir o script abaixo em um Content Editor na página onde o efeito de aumentar o tamanho do campo multiseleção no componente desejado.

<script src="/Remuneracoes/SiteAssets/Scripts/jquery-1.8.3.min.js" type="text/javascript"></script>
<script src="/Remuneracoes/SiteAssets/Scripts/jquery.SPServices-0.7.2.min.js" type="text/javascript"></script>


<script language="javascript" type="text/javascript">

$(document).ready(function() {
  $().SPServices.SPSetMultiSelectSizes({
    multiSelectColumn: "Centros de custo"
  });
});</script>
