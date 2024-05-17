export function timeAgo(prevDate) {
  const diff = Number(new Date()) - prevDate;
  const minute = 60 * 1000;
  const hour = minute * 60;
  const day = hour * 24;
  const month = day * 30;
  const year = day * 365;
  switch (true) {
      case diff < minute:
          const seconds = Math.round(diff / 1000);
           return `${seconds} ${seconds > 1 ? 'seconds' : 'second'} ago`
      case diff < hour:
          return Math.round(diff / minute) + ' minute(s) ago';
      case diff < day:
          return Math.round(diff / hour) + ' hour(s) ago';
      case diff < month:
          return Math.round(diff / day) + ' day(s) ago';
      case diff < year:
          return Math.round(diff / month) + ' month(s) ago';
      case diff > year:
          return Math.round(diff / year) + ' year(s) ago';
      default:
          return "";
  }
}