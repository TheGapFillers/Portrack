${clientId} = "web"
${clientSecret} = ""
${UserName} = "ambroise.couissin@gmail.com"
${Password} = ""
$authToken = invoke-restmethod -Method Post -uri http://localhost:24717/Token -ContentType "application/x-www-form-urlencoded;charset=UTF-8" -Body "grant_type=password&username=${UserName}&password=${Password}&client_id=${clientId}&client_secret=${clientSecret}"
${token_type} = $authToken.token_type
${access_token} = $authToken.access_token

Invoke-RestMethod -Method Get -Uri http://localhost:24717/portfolios/ -Headers @{"Authorization"= "${token_type} ${access_token}"}


$portfolio ='
{
  "UserName": "ambroise.couissin@gmail.com",
  "PortfolioName": "BambiPortfolio4",
}
'
Invoke-RestMethod -Method Post -Uri http://localhost:24717/portfolios -Body $portfolio -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"}

$transaction ='
{
  "portfolioName":"BambiPortfolio",
  "ticker": "goog",
  "shareAmount": -2,
  "date":"2014-03-04",
}
'
Invoke-RestMethod -Method Post -Uri http://localhost:24717/transactions -Body $transaction -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"}


