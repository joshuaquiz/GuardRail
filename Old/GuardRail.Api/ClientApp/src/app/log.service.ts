import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private level: LogLevel = LogLevel.All;
  private logWithDate = true;

  public debug(msg: string, ...optionalParams: any[]): void {
    this.writeToLog(msg, LogLevel.Debug, optionalParams);
  }

  public info(msg: string, ...optionalParams: any[]): void {
    this.writeToLog(msg, LogLevel.Info, optionalParams);
  }

  public warn(msg: string, ...optionalParams: any[]): void {
    this.writeToLog(msg, LogLevel.Warn, optionalParams);
  }

  public error(msg: string, ...optionalParams: any[]): void {
    this.writeToLog(msg, LogLevel.Error, optionalParams);
  }

  public fatal(msg: string, ...optionalParams: any[]): void {
    this.writeToLog(msg, LogLevel.Fatal, optionalParams);
  }

  private writeToLog(msg: string, level: LogLevel, params: any[]): void {
    if (this.shouldLog(level)) {
      let value = '';
      if (this.logWithDate) {
        value = new Date() + ' - ';
      }

      value += `Type: ${LogLevel[level]}`;
      value += ` - Message: ${msg}`;
      if (params.length) {
        value += ` - Extra Info: ${this.formatParams(params)}`;
      }

      console.log(value);
    }
  }

  private shouldLog(level: LogLevel): boolean {
    let ret = false;
    if ((level >= this.level && level !== LogLevel.Off)
      || this.level === LogLevel.All) {
      ret = true;
    }

    return ret;
  }

  private formatParams(params: any[]): string {
    let ret = params.join(',');
    if (params.some(p => typeof p == 'object')) {
      ret = '';
      for (let item of params) {
        ret += JSON.stringify(item) + ',';
      }
    }

    return ret;
  }
}

export enum LogLevel {
  All = 0,
  Debug = 1,
  Info = 2,
  Warn = 3,
  Error = 4,
  Fatal = 5,
  Off = 6
}
