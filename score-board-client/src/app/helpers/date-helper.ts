export class DateHelper {
  constructor() { }

  public static GetDate(dateString: string) {
    const splitDateTime = dateString.toString().split(' ');
    const splitDate = splitDateTime[0].split('.');
    return new Date(splitDate[2] + '-' + splitDate[1] + '-' + splitDate[0] + 'T' + splitDateTime[1]);
  }
}
