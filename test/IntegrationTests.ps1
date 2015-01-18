${clientId} = "web"
${clientSecret} = ""
${UserName} = "ambroise.couissin@gmail.com"
${Password} = ""
$authToken = invoke-restmethod -Method Post -uri http://localhost:24717/Token -ContentType "application/x-www-form-urlencoded;charset=UTF-8" -Body "grant_type=password&username=${UserName}&password=${Password}&client_id=${clientId}&client_secret=${clientSecret}"
${token_type} = $authToken.token_type
${access_token} = $authToken.access_token

$authToken.access_token | Out-File "C:\Users\Ambroise\Dev\aaa.txt"

Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/portfolios/ -Headers @{"Authorization"= "${token_type} ${access_token}"}


$portfolio ='
{
  "UserName": "ambroise.couissin@gmail.com",
  "PortfolioName": "BambiPortfolio1",
}
'
Invoke-RestMethod -Method Post -Uri http://localhost:24717/api/portfolios -Body $portfolio -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"}

$transaction ='
{
  "portfolioName":"BambiPortfolio1",
  "ticker": "goog",
  "shares": 2,
  "date":"2014-03-04",
  "type":"Buy"
}
'
Invoke-RestMethod -Method Post -Uri http://localhost:24717/api/transactions -Body $transaction -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"}


