export class User {
  constructor(
    public firstName: string,
    public lastName: string,
    public email: string,
    public id: string,
    private _token: string,
    private _tokenExpirationDate: Date,
    private _refreshToken: string
  ) {}

  get token() {
    if (!this._tokenExpirationDate || new Date() > this._tokenExpirationDate) {
      return null;
    }
    return this._token;
  }

  get tokenExpirationDate() {
    if (!this._tokenExpirationDate || new Date() > this._tokenExpirationDate) {
      return null;
    }
    return this._tokenExpirationDate;
  }

  get refreshToken() {
    return this._refreshToken;
  }
}
