import { AccessPointType } from "./access-point-type";

export interface IAccessPoint {
  Guid: string;

  LocationGuid: string;

  Name: string;

  AccessPointType: AccessPointType;

  Latitude: number | null;

  Longitude: number | null;

  GeoFenceDistance: number | null;

  RequiresAllAccessMethods: boolean;

  AccessMethodTimeout: string | null;

  IsLocked: boolean;

  IsOpen: boolean;
}
