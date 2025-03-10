export class CreateLocationRequest {
  public constructor(
    accountId: string,
    name: string,
    desctiption: string | null,
    latitude: number | null,
    longitude: number | null,
    geoFenceDistance: number | null,
    isMobile: boolean) {
    this.AccountId = accountId;
    this.Name = name;
    this.Desctiption = desctiption;
    this.Latitude = latitude;
    this.Longitude = longitude;
    this.GeoFenceDistance = geoFenceDistance;
    this.IsMobile = isMobile;
  }

  public AccountId: string;

  public Name: string;

  public Desctiption: string | null;

  public Latitude: number | null;

  public Longitude: number | null;

  public GeoFenceDistance: number | null;

  public IsMobile: boolean;
}
