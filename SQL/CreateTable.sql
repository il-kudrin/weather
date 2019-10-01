
CREATE TABLE WeatherItems 
 (
   Id int NOT NULL AUTO_INCREMENT,
   CityName nvarchar(100),
   Date Date,
   Discription nvarchar(500),
   MinT int,
   MaxT int,
   Primary key (Id),
   Index (CityName, Date),
   constraint uq_CityDate Unique(CityName, Date)
 )
