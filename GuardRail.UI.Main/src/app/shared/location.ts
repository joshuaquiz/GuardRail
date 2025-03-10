export interface ILocation {
  Guid: string;

  AccountId: string;

  Name: string;

  Description: string | null;

  Latitude: number | null;

  Longitude: number | null;

  GeoFenceDistance: number | null;

  IsMobile: boolean;
}
