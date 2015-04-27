Invoke-Restmethod -Method Post -Uri http://localhost:8282/api/audiences -ContentType "application/json" -Body "{ 'name' : 'Portrack' }"

${clientId} = "14631eb2-6edc-e411-8398-e0b9a567eb25"
${clientSecret} = "bcMTRsTDRszM19MKhbsRbbfRjVzDZJ4lxGbc3iZfiJ8"
${UserName} = "ambroise.couissin@gmail.com"
${Password} = "Aaaa-1111"
$authToken = invoke-restmethod -Method Post -uri http://localhost:24717/Token -ContentType "application/x-www-form-urlencoded;charset=UTF-8" -Body "grant_type=password&username=${UserName}&password=${Password}&client_id=${clientId}&client_secret=${clientSecret}" -Verbose
${token_type} = $authToken.token_type
${access_token} = $authToken.access_token

Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/portfolios/ -Headers @{"Authorization"= "${token_type} ${access_token}"}
Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/portfolios/BambiPortfolio1 -Headers @{"Authorization"= "${token_type} ${access_token}"}


Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/transactions/ -Headers @{"Authorization"= "${token_type} ${access_token}"}
Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/transactions/BambiPortfolio1 -Headers @{"Authorization"= "${token_type} ${access_token}"}
Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/transactions/BambiPortfolio1/MSFT -Headers @{"Authorization"= "${token_type} ${access_token}"}

Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/holdings/BambiPortfolio1 -Headers @{"Authorization"= "${token_type} ${access_token}"} | Format-List
Invoke-RestMethod -Method Get -Uri "http://localhost:24717/api/holdings/BambiPortfolio1/MSFT,GOOG" -Headers @{"Authorization"= "${token_type} ${access_token}"} | Format-List

Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/instruments/ -Headers @{"Authorization"= "${token_type} ${access_token}"}
Invoke-RestMethod -Method Get -Uri "http://localhost:24717/api/instruments/GOOG,YHOO" -Headers @{"Authorization"= "${token_type} ${access_token}"}
Invoke-RestMethod -Method Get -Uri "http://localhost:24717/api/instruments/MSFT" -Headers @{"Authorization"= "${token_type} ${access_token}"}


$portfolio ='
{
  "PortfolioName": "BambiPortfolio1",
}
'
Invoke-RestMethod -Method Post -Uri http://localhost:24717/api/portfolios -Body $portfolio -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"} 


Invoke-RestMethod -Method Delete -Uri http://localhost:24717/api/portfolios/BambiPortfolio1 -Headers @{"Authorization"= "${token_type} ${access_token}"}

#Then following must return null
Invoke-RestMethod -Method Get -Uri http://localhost:24717/api/portfolios/BambiPortfolio1 -Headers @{"Authorization"= "${token_type} ${access_token}"}

$transaction ='
{
  "portfolioName":"BambiPortfolio1",
  "ticker": "goog",
  "shares": 2,
  "date":"2014-04-18",
  "type":"Buy"
}
'
$transaction ='
{
  "portfolioName":"BambiPortfolio1",
  "ticker": "goog",
  "shares": 2,
  "date":"2014-04-21",
  "type":"Buy"
}
'
$transaction ='
{
  "portfolioName":"BambiPortfolio1",
  "ticker": "msft",
  "shares": 2,
  "date":"2015-02-11",
  "type":"Buy"
}
'

Invoke-RestMethod -Method Post -Uri http://localhost:24717/api/transactions -Body $transaction -ContentType "application/json" -Headers @{"Authorization"= "${token_type} ${access_token}"}

