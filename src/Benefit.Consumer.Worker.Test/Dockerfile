FROM node:alpine
EXPOSE 3000
COPY Benefit.Consumer.Worker.Test/Mock/db.json /opt/mock.json
#COPY Benefit.Consumer.Worker.Test/entry.sh /app/entry.sh
#RUN chmod +x /app/entry.sh
RUN yarn global add json-server

#ENTRYPOINT ["/app/entry.sh"]
ENTRYPOINT ["json-server", "-H", "0.0.0.0", "/opt/mock.json"]
#CMD ["json-server", "-H", "0.0.0.0", "/opt/mock.json"]